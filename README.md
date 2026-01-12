# Metalian Suit

Um jogo de plataforma 2D estilo NES desenvolvido em Godot 4 com C#.

## 🎮 Visão Geral

Metalian Suit é um metroidvania sidescroller que captura a essência dos jogos clássicos de NES, com física precisa, combate estratégico e exploração procedural.

## 🏗️ Arquitetura

### Sistema NES-Style Core
- **Constraint Core**: Regras imutáveis que garantem fidelidade NES
- **Frame Intent**: Sistema de entrada baseado em frames
- **State Machine**: Gerenciamento de estados determinístico
- **Entity System**: Hierarquia clara Player/Enemy/Entity

### Estrutura de Pastas
```
├── nes_core/           # Sistema core do jogo
│   ├── autoload/       # Singletons globais
│   ├── components/     # Componentes reutilizáveis
│   ├── core/          # Classes base
│   ├── data/          # Recursos de dados
│   ├── managers/      # Gerenciadores de sistema
│   └── stages/        # Sistema de fases
├── entities/          # Entidades do jogo
│   ├── player/        # Jogador e estados
│   └── enemies/       # Inimigos
├── assets/            # Recursos visuais
└── docs/              # Documentação
```

## 🚀 Como Executar

### Pré-requisitos
- Godot 4.2+
- .NET 6.0+

### Setup
1. Clone o repositório
2. Abra o projeto no Godot Editor
3. Execute a cena `nes_core/ui/TitleScreen.tscn`

## 🎯 Características Técnicas

### Sistema de Física NES-Style
- Movimento determinístico
- Controle aéreo limitado (opcional via ConstraintCore)
- Sistema de knockback preciso

### Sistema de Combate
- Ataques com startup/active/recovery
- Sistema de invulnerabilidade
- Dano por contato e hitboxes

### Gerenciamento de Estados
- Máquina de estados hierárquica
- Estados compostáveis
- Transições determinísticas

## 📚 Documentação

Para documentação completa do sistema, consulte:
- [docs/README.md](docs/README.md) - Documentação técnica completa
- [docs/CONTRIBUTING.md](docs/CONTRIBUTING.md) - Guia de contribuição

## 🛠️ Ferramentas de Desenvolvimento

- `tools/build.ps1` - Script de build automatizado
- `tools/editor_settings.txt` - Configurações recomendadas do editor

## 🎨 Assets

- **Sprites**: Pixel art 16x16 otimizado para NES
- **Paleta**: Paleta de cores NES autêntica
- **Fonte**: Fonte pixel perfeita

## 🔧 Desenvolvimento

### Convenções de Código
- C# 10+ com pattern matching
- PascalCase para classes e métodos
- camelCase para variáveis locais
- Prefixo `I` para interfaces

### Testes
- Sistema de debug integrado (ConstraintCore.ShowHitboxes)
- Logs detalhados para diagnóstico
- Asserts em pontos críticos

## 🎮 Controles

- **A**: Pular
- **B**: Atacar
- **Setas**: Movimento
- **Start**: Menu/Pause
- **Select**: (Reservado)

## 📝 Licença

Este projeto é desenvolvido para fins educacionais e de demonstração.

## 🙏 Agradecimentos

Inspirado nos clássicos de NES como Mega Man, Metroid e Castlevania.