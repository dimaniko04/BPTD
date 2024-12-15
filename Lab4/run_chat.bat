@echo off
if not exist server.py (
    echo Error: server.py not found in the current directory.
    exit /b
)
if not exist client.py (
    echo Error: client.py not found in the current directory.
    exit /b
)

start cmd /k python server.py

timeout /t 4 >nul

start cmd /c python client.py
timeout /t 2 >nul
start cmd /c python client.py
timeout /t 2 >nul
start cmd /c python client.py

echo Server and three clients launched in separate Command Prompt windows.
