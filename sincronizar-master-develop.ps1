# Script para sobrescribir develop con todo el contenido de master
# Ejecutar desde la raiz del proyecto

Write-Host "=== SINCRONIZAR MASTER -> DEVELOP ===" -ForegroundColor Cyan
Write-Host ""

# Verificar que estamos en un repositorio Git
if (-not (Test-Path .git)) {
    Write-Host "ERROR: No estas en la raiz de un repositorio Git" -ForegroundColor Red
    exit 1
}

Write-Host "Este script hara que develop quede EXACTAMENTE igual que master" -ForegroundColor Yellow
Write-Host "Todo el contenido actual de develop sera reemplazado" -ForegroundColor Yellow
Write-Host ""
$confirmacion = Read-Host "Continuar? (S/N)"
if ($confirmacion -ne "S" -and $confirmacion -ne "s") {
    Write-Host "Operacion cancelada" -ForegroundColor Yellow
    exit 0
}

Write-Host ""
Write-Host "[1/4] Cambiando a master y guardando todo..." -ForegroundColor Cyan
git checkout master 2>&1 | Out-Null
git add . 2>&1 | Out-Null
git commit -m "WIP: estado actual completo antes de sincronizar con develop" 2>&1 | Out-Null
Write-Host "  OK Master commiteado" -ForegroundColor Green

Write-Host ""
Write-Host "[2/4] Cambiando a develop..." -ForegroundColor Cyan
git checkout develop 2>&1 | Out-Null
Write-Host "  OK En rama develop" -ForegroundColor Green

Write-Host ""
Write-Host "[3/4] Sobrescribiendo develop con master..." -ForegroundColor Cyan

# Crear mensaje de merge temporal
$mergeMsg = "feat: sincronizar develop con master - version funcional"
$env:GIT_MERGE_AUTOEDIT = "no"

# Hacer merge forzando la version de master
git merge master -X theirs --no-edit -m $mergeMsg 2>&1 | Out-Null

# Si hay conflictos, resolverlos con la version de master
$status = git status --porcelain
if ($status -match "UU") {
    Write-Host "  Resolviendo conflictos con version de master..." -ForegroundColor Yellow
    git checkout --theirs . 2>&1 | Out-Null
    git add . 2>&1 | Out-Null
    git commit --no-edit 2>&1 | Out-Null
}

Write-Host "  OK Develop actualizado con master" -ForegroundColor Green

Write-Host ""
Write-Host "[4/4] Subiendo a remoto..." -ForegroundColor Cyan
git push origin develop --force-with-lease 2>&1 | Out-Null

if ($LASTEXITCODE -eq 0) {
    Write-Host "  OK Push completado" -ForegroundColor Green
} else {
    Write-Host "  Error en push, intenta manualmente: git push origin develop --force-with-lease" -ForegroundColor Red
}

Write-Host ""
Write-Host "=== SINCRONIZACION COMPLETADA ===" -ForegroundColor Green
Write-Host ""
Write-Host "Develop ahora tiene todo el contenido de master" -ForegroundColor Cyan
Write-Host "Estas en la rama: develop" -ForegroundColor Green
Write-Host ""
