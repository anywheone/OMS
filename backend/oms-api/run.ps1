Set-Location $PSScriptRoot
$env:JAVA_HOME = "C:\Program Files\Eclipse Adoptium\jdk-17.0.16.8-hotspot"
$env:PATH = "$env:JAVA_HOME\bin;$env:PATH"
& ".\mvnw.cmd" "spring-boot:run" "-Dspring-boot.run.profiles=dev"
