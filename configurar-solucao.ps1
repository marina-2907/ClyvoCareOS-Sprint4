$ErrorActionPreference = 'Stop'
$origem = $PSScriptRoot
$projeto = (Get-Location).Path
if (-not (Test-Path (Join-Path $projeto 'ClyvoCareOSChallenge.csproj'))) {
    throw 'Abra o PowerShell NA PASTA que contem ClyvoCareOSChallenge.csproj antes de executar.'
}
$destinoBackup = Join-Path (Split-Path $projeto -Parent) ('backup-clean-' + (Get-Date -Format 'yyyyMMdd-HHmmss'))
New-Item -ItemType Directory -Path $destinoBackup -Force | Out-Null
$arquivos = Get-ChildItem $origem -Recurse -Filter '*.cs' -File
$programa = Join-Path $projeto 'Program.cs'
Copy-Item $programa (Join-Path $destinoBackup 'Program.cs')
foreach ($arquivo in $arquivos) {
    $rel = $arquivo.FullName.Substring($origem.Length).TrimStart('\','/')
    $target = Join-Path $projeto $rel
    if (Test-Path $target) {
        $bk = Join-Path $destinoBackup $rel
        New-Item -ItemType Directory (Split-Path $bk -Parent) -Force | Out-Null
        Copy-Item $target $bk -Force
    }
    New-Item -ItemType Directory (Split-Path $target -Parent) -Force | Out-Null
    Copy-Item $arquivo.FullName $target -Force
}
$conteudo = Get-Content -Raw $programa
if ($conteudo -notmatch 'AddScoped<ClyvoCareOSChallenge.Application.Contracts.ITutorRepository') {
    $registro = Get-Content -Raw (Join-Path $origem 'REGISTROS-PROGRAM.txt')
    if ($conteudo -notmatch 'var app = builder.Build\(\);') {
        throw 'Nao localizei a linha var app = builder.Build();. Arquivos anteriores estao salvos no backup.'
    }
    $conteudo = $conteudo.Replace('var app = builder.Build();', $registro + [Environment]::NewLine + '        var app = builder.Build();')
    Set-Content -Path $programa -Value $conteudo -Encoding UTF8
}
Write-Host "Backup: $destinoBackup" -ForegroundColor Cyan
Write-Host 'Arquivos copiados e servicos registrados. Testando...' -ForegroundColor Green
dotnet build
if ($LASTEXITCODE -ne 0) { throw "Compilacao falhou. Consulte o log. Backup em $destinoBackup" }
dotnet test --no-build
if ($LASTEXITCODE -ne 0) { throw "Algum teste falhou. Consulte o log. Backup em $destinoBackup" }
Write-Host 'BUILD E TESTES CONCLUIDOS!' -ForegroundColor Green
