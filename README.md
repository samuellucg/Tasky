# Tasky 📝

> Interface desktop WPF que espelha em tempo real as tarefas gerenciadas via **Telegram Bot**

![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.7-blue)
![WPF](https://img.shields.io/badge/WPF-Desktop-purple)
![Socket.IO](https://img.shields.io/badge/Socket.IO-Real--time-green)
![Telegram](https://img.shields.io/badge/Telegram-Bot_API-26A5E4)

---

## O que é o Tasky?

O **Tasky** é uma aplicação desktop que serve como **visualização estendida** das tarefas criadas e gerenciadas através de um Bot no Telegram. Em vez de consultar tarefas apenas pelo chat, você tem uma interface visual completa com:

- 📋 Lista organizada de tarefas
- 🔔 Notificações visuais de lembretes
- ✏️ Edição inline rápida
- 🗑️ Exclusão com confirmação

**O diferencial?** Tudo sincronizado em **tempo real** via WebSocket. Quando você cria, edita ou deleta uma tarefa pelo Telegram, a interface desktop atualiza instantaneamente — e vice-versa.

---

## 🏗️ Arquitetura

```
┌─────────────────┐      WebSocket      ┌─────────────────┐
│   Telegram Bot  │ ◄──────────────────► │   API-TASKY     │
│   (Usuário)     │                      │   (Backend)     │
└─────────────────┘                      └────────┬────────┘
                                                  │
                                                  │ HTTP / WebSocket
                                                  ▼
                                         ┌─────────────────┐
                                         │    Tasky        │
                                         │   (WPF App)     │
                                         │  Interface      │
                                         │  Desktop        │
                                         └─────────────────┘
```

### Repos relacionados

- 🔗 **[API-Tasky](https://github.com/samuellucg/API-Tasky)** — back-end Node.js com PostgreSQL, Redis e bot do Telegram
- 🤖 **Bot Telegram** — Gerenciamento via chat (integrado no repo acima)

---

## ⚡ Funcionalidades

| Feature | Descrição |
|---------|-----------|
| **Sincronização Real-time** | Alterações no Telegram refletem instantaneamente na interface desktop via WebSocket |
| **CRUD Completo** | Crie, leia, atualize e delete tarefas diretamente pelo desktop |
| **Edição Inline** | Duplo-clique para editar nome, descrição, data e horário sem modais |
| **Notificações Inteligentes** | Alertas visuais 15min e 5min antes do horário da tarefa |
| **Reconexão Automática** | Socket reconecta automaticamente se a conexão cair |
| **Recovery de Erros** | Se a API estiver offline, o app reinicia automaticamente |

---

## 🛠️ Stack Técnico

- **.NET Framework 4.7** + **WPF** — Interface rica e responsiva
- **MVVM Pattern** — Separação de concerns com `INotifyPropertyChanged`
- **Socket.IO Client** — Comunicação bidirecional em tempo real
- **HttpClient** — REST API para persistência
- **NLog** — Logging estruturado em arquivo
- **Extended WPF Toolkit** — Componentes avançados (MaskedTextBox, etc.)

---

## 📁 Estrutura do Projeto

```
Tasky/
├── Models/
│   └── Task.cs              # Entidade com notificações de lembrete
│   └── UserOp.cs            # Operações do usuário
├── ViewModels/
│   └── BaseViewModel.cs     # Implementação base de INotifyPropertyChanged
│   └── MainViewModel.cs     # Lógica principal + binding
├── Views/
│   └── TasksHomePage.xaml   # Interface principal (ListView customizada)
│   └── Utils/               # Modais de adição, confirmação, alerta
├── Services/
│   └── Socket/              
│       └── SocketClient.cs  # Cliente Socket.IO estático com eventos
├── Database/
│   └── Database.cs          # HttpClient wrapper para API REST
└── Common/
    └── Converters/          # Conversores de visibilidade para MVVM
```

---

## 🚀 Como Executar

### Pré-requisitos

- Visual Studio 2019+ ou VS Code com extensão C#
- .NET Framework 4.7
- API-TASKY rodando em `localhost:3000`

### Passos

```bash
# Clone o repositório
git clone https://github.com/samuellucg/Tasky.git

# Restore packages (NuGet)
nuget restore Tasky.sln

# Build
msbuild Tasky.sln

# Execute (certifique-se que a API está rodando)
./bin/Debug/Tasky.exe
```

---

## 🔄 Fluxo de Sincronização

1. Usuário cria tarefa via Telegram Bot → API recebe → Emite evento WebSocket
2. Tasky escuta o evento `HasChangedEvent` → Recarrega lista automaticamente
3. Usuário edita tarefa no Tasky → PUT para API → API notifica Telegram
4. Ciclo bidirecional mantém tudo sincronizado

---

## 💡 Aprendizados & Evolução

Decisões técnicas e desafios enfrentados na construção:

- Threading em WPF (`Dispatcher.Invoke` para updates de UI)
- Async/await patterns em aplicações desktop
- Implementação de MVVM sem frameworks (pure code)
- Socket.IO client em .NET Framework

### O que faria diferente hoje:

- **Injeção de dependência** — atualmente há acoplamento direto
- **Configuração externa** — URLs hardcoded poderiam vir de appsettings
- **Testes unitários** — interfaces facilitariam mocks
- **Separação de concerns** — Database.cs mistura HTTP com lógica de restart
