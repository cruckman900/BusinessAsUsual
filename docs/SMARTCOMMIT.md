## 🎸 Smart Commit Convention Guide
🧠 Purpose
Smart Commit tags help:
- Automate changelog generation
- Trigger semantic version bumps
- Categorize contributor impact
- Make your Git history expressive and searchable

## 📝 Format
<emoji> <type>(scope): description [optional tags]

## 🔧 Anatomy
- Emoji: Adds visual clarity and contributor flair
- Type: Semantic tag (e.g., feat, fix, chore)
- Scope: Optional module or feature (e.g., admin, auth)
- Description: Clear, concise summary
- Tags: Optional hashtags for filtering (#infra, #tests, #branding)

## 🔖 Common Smart Commit Types

| Emoji   | Tag         | Purpose                                  | Changelog Section      | Version Bump |
|---------|-------------|------------------------------------------|------------------------|--------------|
| ✨     | `feat:`     | New feature                              | 🟢 Features            | Minor        |
| 🐛     | `fix:`      | Bug fix                                  | 🔧 Bug Fixes           | Patch        |
| 🧹     | `chore:`    | Maintenance, tooling, cleanup            | 🧹 Chores              | None         |
| 📚     | `docs:`     | Documentation changes                    | 📚 Documentation       | None         |
| 🔄     | `refactor:` | Code restructuring (no behavior change)  | 🔄 Refactoring         | None         |
| 🧪     | `test:`     | Adding or updating tests                 | 🧪 Tests               | None         |
| 🚀     | `perf:`     | Performance improvements                 | 🚀 Performance         | Patch        |
| ⚙️     | `ci:`       | CI/CD pipeline changes                   | ⚙️ CI/CD               | None         |
| 🎨     | `style:`    | Formatting, whitespace, etc.             | 🎨 Style               | None         |
| 🔒     | `security:` | Vulnerability patches                    | 🔐 Security            | Patch        |
| 🧭     | `build:`    | Build system changes                     | 🧭 Build               | None         |
| 🧱     | `infra:`    | Infrastructure orchestration             | 🧱 Infrastructure     

For a full list, check out Conventional Commits: https://www.conventionalcommits.org/en/v1.0.0/

## 🧪 Examples
✨ feat(admin): add user provisioning flow #infra #aws
🐛 fix(auth): resolve login redirect loop #security
📚 docs(onboarding): update contributor guide with Smart Commit examples
🧪 test(api): add coverage for billing endpoint #tests
🔄 refactor(layout): modularize theme config #branding

## 🧠 Bonus: Commit Metadata
You can inject metadata into your commits for:
- Changelog grouping
- Contributor tagging
- Release note enrichment
Examples:
📝 chore(release): v0.1.1 [skip ci]
✨ feat(admin): add user provisioning flow #infra #aws @cruckman900

Tools like conventional-changelog-action will pick up:
- feat: → Features
- fix: → Bug Fixes
- chore: → Maintenance
- @username → Contributor mentions
- #tag → Custom filters

## 🧭 Best Practices
- Keep descriptions short but expressive
- Use emojis to visually group commits
- Use scopes to clarify affected modules
- Use tags to track contributor themes
- Avoid vague commits like update stuff or fix bug
