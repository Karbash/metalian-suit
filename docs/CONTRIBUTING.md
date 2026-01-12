# Contribuindo para Metalian Suit

Obrigado por seu interesse em contribuir com o Metalian Suit! Este documento estabelece as diretrizes para contribuições.

## 🐛 Relatando Bugs

1. Verifique se o bug já foi reportado
2. Use o template de issue para bugs
3. Inclua:
   - Versão do Godot
   - Sistema operacional
   - Passos para reproduzir
   - Comportamento esperado vs atual

## ✨ Solicitando Features

1. Verifique se a feature já foi solicitada
2. Descreva claramente o problema que resolve
3. Considere alternativas já existentes
4. Mantenha o foco em features NES-style

## 💻 Contribuindo Código

### Pré-requisitos
- Godot 4.2+
- .NET 6.0+
- Familiaridade com C# e Godot

### Processo de Desenvolvimento

1. **Fork** o repositório
2. **Clone** seu fork: `git clone https://github.com/YOUR_USERNAME/metalian-suit.git`
3. **Crie uma branch**: `git checkout -b feature/nome-da-feature`
4. **Faça suas mudanças** seguindo as convenções abaixo
5. **Teste** suas mudanças
6. **Commit**: `git commit -m "feat: descrição clara"`
7. **Push**: `git push origin feature/nome-da-feature`
8. **Abra um Pull Request**

### Convenções de Código

#### C#
```csharp
// ✅ BOM
public class PlayerState : State
{
    protected override string AnimationName => "idle";

    public override void Update(double delta)
    {
        base.Update(delta);
        // Lógica aqui
    }
}

// ❌ RUIM
public class playerstate : state
{
    protected override string AnimationName => "idle";
    public override void Update(double delta) { base.Update(delta); /* código aqui */ }
}
```

#### Nomenclatura
- **Classes**: `PascalCase` (ex: `PlayerState`)
- **Métodos**: `PascalCase` (ex: `UpdatePhysics()`)
- **Variáveis**: `camelCase` (ex: `currentVelocity`)
- **Constantes**: `UPPER_CASE` (ex: `MAX_HEALTH`)
- **Interfaces**: Prefixo `I` (ex: `IStateMachine`)

#### Estrutura de Arquivos
```
nes_core/
├── component/
│   ├── ComponentName.cs
│   └── ComponentName.tscn (se necessário)
├── core/
│   └── BaseClass.cs
└── feature/
    ├── Feature.cs
    └── subfeature/
        └── SubFeature.cs
```

### Testes
- Teste suas mudanças em diferentes cenários
- Use o `ConstraintCore.ShowHitboxes` para debug visual
- Verifique se não quebrou funcionalidades existentes

### Commits
- Use commits pequenos e descritivos
- Siga Conventional Commits:
  - `feat:` novas funcionalidades
  - `fix:` correções de bug
  - `docs:` documentação
  - `refactor:` refatoração
  - `test:` testes

### Pull Requests
- Título claro e descritivo
- Descrição detalhada das mudanças
- Referências a issues relacionadas
- Screenshots/videos se aplicável
- Aguarde revisão da equipe

## 🎨 Contribuindo Assets

### Sprites
- Use pixel art 16x16
- Mantenha consistência com a paleta NES
- Teste animações no engine

### Áudio
- Formato OGG preferido
- Mantenha tamanho pequeno
- Teste sincronização com animações

## 📚 Documentação

- Atualize README.md se necessário
- Documente novas APIs
- Mantenha docs/ atualizado

## ❓ Precisa de Ajuda?

- Abra uma issue no GitHub
- Consulte a documentação em docs/
- Veja exemplos no código existente

## 📋 Checklist de Pull Request

- [ ] Código compila sem erros
- [ ] Segue convenções de código
- [ ] Inclui testes apropriados
- [ ] Atualiza documentação
- [ ] Funciona em Windows/Linux/Mac
- [ ] Não quebra funcionalidades existentes

Obrigado por contribuir com o Metalian Suit! 🎮