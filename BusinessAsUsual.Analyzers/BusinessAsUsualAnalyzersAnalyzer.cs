using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BusinessAsUsual.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DocumentationTagAnalyzer : DiagnosticAnalyzer
    {
        private static readonly DiagnosticDescriptor MissingAuthorTagRule = new DiagnosticDescriptor(
            id: "BAU001",
            title: "Missing <author> tag in XML documentation",
            messageFormat: "The XML documentation for '{0}' is missing an <author> tag.",
            category: "Documentation",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "All public methods should include an <author> tag in their XML documentation.");

        private static readonly DiagnosticDescriptor MissingLastModifiedByTagRule = new DiagnosticDescriptor(
            id: "BAU002",
            title: "Missing <lastmodifiedby> tag in XML documentation",
            messageFormat: "The XML documentation for '{0}' is missing a <lastmodifiedby> tag.",
            category: "Documentation",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "All public methods should include a <lastmodifiedby> tag in their XML documentation.");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(MissingAuthorTagRule, MissingLastModifiedByTagRule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
        }

        private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDecl = (MethodDeclarationSyntax)context.Node;

            // Only analyze public methods
            if (!methodDecl.Modifiers.Any(SyntaxKind.PublicKeyword))
            {
                return;
            }

            var trivia = methodDecl.GetLeadingTrivia()
                .Select(t => t.GetStructure())
                .OfType<DocumentationCommentTriviaSyntax>()
                .FirstOrDefault();

            if (trivia == null)
            {
                return;
            }

            var xmlText = trivia.ToFullString();

            if (!xmlText.Contains("<author>"))
            {
                var diagnostic = Diagnostic.Create(
                    MissingAuthorTagRule,
                    methodDecl.Identifier.GetLocation(),
                    methodDecl.Identifier.Text);
                context.ReportDiagnostic(diagnostic);
            }

            if (!xmlText.Contains("<lastmodifiedby>"))
            {
                var diagnostic = Diagnostic.Create(
                    MissingLastModifiedByTagRule,
                    methodDecl.Identifier.GetLocation(),
                    methodDecl.Identifier.Text);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}