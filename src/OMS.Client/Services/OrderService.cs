using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using OMS.Client.Models;
using Microsoft.Extensions.Logging;

namespace OMS.Client.Services;

/// <summary>
/// 注文APIサービス
/// </summary>
public class OrderService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OrderService>? _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public OrderService(HttpClient httpClient, ILogger<OrderService>? logger = null)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    /// <summary>
    /// 注文一覧を取得
    /// </summary>
    public async Task<List<OrderModel>> GetOrdersAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/orders");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger?.LogError("注文一覧取得エラー: StatusCode={StatusCode}, Content={Content}",
                    response.StatusCode, errorContent);
                throw new Exception($"注文一覧の取得に失敗しました: {response.StatusCode}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            _logger?.LogInformation("注文一覧レスポンス: {Response}", responseContent);

            // ApiResponse<List<OrderDto>>の形式でレスポンスが返ってくる
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseList>(_jsonOptions);

            if (apiResponse?.Data == null)
            {
                return new List<OrderModel>();
            }

            var orders = System.Text.Json.JsonSerializer.Deserialize<List<OrderModel>>(
                apiResponse.Data.ToString() ?? "[]", _jsonOptions);

            return orders ?? new List<OrderModel>();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "注文一覧の取得に失敗しました");
            throw;
        }
    }

    /// <summary>
    /// 新規注文を作成
    /// </summary>
    public async Task<OrderModel> CreateOrderAsync(OrderModel order)
    {
        try
        {
            // CreateOrderDto形式に変換
            var createOrderDto = new
            {
                securityId = order.SecurityId,
                side = order.Side,
                orderType = order.OrderType,
                quantity = order.Quantity,
                price = order.Price,
                stopPrice = order.StopPrice,
                timeInForce = order.TimeInForce,
                validUntil = order.ValidUntil,
                notes = order.Notes
            };

            var response = await _httpClient.PostAsJsonAsync("/api/orders", createOrderDto);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger?.LogError("注文作成エラー: StatusCode={StatusCode}, Content={Content}",
                    response.StatusCode, errorContent);
                throw new Exception($"注文の作成に失敗しました: {response.StatusCode}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            _logger?.LogInformation("注文作成レスポンス: {Response}", responseContent);

            // ApiResponse<OrderDto>の形式でレスポンスが返ってくる
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>(_jsonOptions);

            if (apiResponse?.Data == null)
            {
                throw new Exception("注文の作成に失敗しました: データが空です");
            }

            var result = System.Text.Json.JsonSerializer.Deserialize<OrderModel>(
                apiResponse.Data.ToString() ?? "{}", _jsonOptions);

            return result ?? throw new Exception("注文の作成に失敗しました");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "注文の作成に失敗しました");
            throw;
        }
    }

    private class ApiResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
    }

    private class ApiResponseList
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
    }

    /// <summary>
    /// 注文をキャンセル
    /// </summary>
    public async Task<bool> CancelOrderAsync(long orderId)
    {
        try
        {
            var response = await _httpClient.PutAsync($"/api/orders/{orderId}/cancel", null);
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "注文のキャンセルに失敗しました: {OrderId}", orderId);
            throw;
        }
    }
}
