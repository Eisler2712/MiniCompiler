using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Compiler.Syntax;

namespace Compiler.Binding
{
    ///<summary>
    /// The Binder class is responsible for binding the syntax tree to the bound tree.
    /// It is responsible for creating the bound tree.
    ///</summary>
    internal sealed partial class Binder
    {
        public DiagnosticBag Diagnostics => _diagnostics;

        private readonly DiagnosticBag _diagnostics = new DiagnosticBag();
        private BoundScope _scope;

        public Binder(BoundScope parent)
        {
            _scope = new BoundScope(parent);
        }

        ///<summary>
        /// Bind the global scope
        ///</summary>
        public static BoundGlobalScope BindGlobalScope(BoundGlobalScope previous, CompilerUnitSyntax syntax)
        {
            var parentScope = CreateParentScopes(previous);
            var binder = new Binder(parentScope);
            var statement = binder.BindStatement(syntax.Statement);
            var variables = binder._scope.GetDeclaredVariables();
            var diagnostics = binder.Diagnostics.ToImmutableArray();

            if(previous != null)
                diagnostics.InsertRange(0,previous.Diagnostics);

            return new BoundGlobalScope(previous, diagnostics,variables,statement);
        }

        ///<summary>
        /// Create the parent scopes
        ///</summary>
        private static BoundScope CreateParentScopes(BoundGlobalScope previous)
        {
            var stack = new Stack<BoundGlobalScope>();
            while (previous != null)
            {
                stack.Push(previous);
                previous = previous.Previous;
            }
            BoundScope parent = null;
            while (stack.Count > 1)
            {
                previous = stack.Pop();
                var scope = new BoundScope(parent);
                foreach (var v in previous.Variables)
                    scope.TryDeclare(v);
                
                parent = scope;
            }
            return parent;
        }

        ///<summary>
        /// Bind the statement
        ///</summary>
        public BoundStatement BindStatement(StatementSyntax syntax)
        {
            switch (syntax.Kind)
            {
                case SyntaxKind.IfStatement:
                    return BindIfStatement((IfStatementSyntax) syntax);
                case SyntaxKind.BlockStatement:
                    return BindBlockStatement((BlockStatementSyntax)syntax);
                case SyntaxKind.ExpressionStatement:
                    return BindExpressionStatement((ExpressionStatementSyntax)syntax);
                default:
                    throw new Exception($"Unexpected syntax {syntax.Kind}");
            }
        }

        ///<summary>
        /// Bind the if statement
        ///</summary>
       private BoundStatement BindIfStatement(IfStatementSyntax syntax)
        {
            var condition = BindExpression(syntax.ParenthesisExpression.Expression,typeof(bool), syntax.IfToken);
            var ifStatement = BindStatement(syntax.BlockIfStatement);
            var elseStatement = syntax.ElseClauseStatement == null ? null : BindStatement(syntax.ElseClauseStatement.ElseStatement);
            return new BoundIfStatement(condition,ifStatement, elseStatement);
        }

        ///<summary>
        /// Bind the expression
        ///</summary>
        private BoundExpression BindExpression(ExpressionSyntax condition, Type targetType, SyntaxToken ifToken)
        {
            var result = BindExpression(condition);
            if(result.Type != targetType)
                _diagnostics.ReportCannotConvert(ifToken.Span,result.Type, targetType);

            return result;    
        }

        ///<summary>
        /// Bind the block statement
        ///</summary>
        private BoundStatement BindBlockStatement(BlockStatementSyntax syntax)
        {
            var statements = new List<BoundStatement>();
            _scope = new BoundScope(_scope);
            foreach (var statementSyntax in syntax.Statements)
            {
                var statement = BindStatement(statementSyntax);
                statements.Add(statement);
            }
            _scope = _scope.Parent;
            return new BoundBlockStatement(statements);
        }

        ///<summary>
        /// Bind the expression statement
        ///</summary>
        private BoundStatement BindExpressionStatement(ExpressionStatementSyntax syntax)
        {
            var expression = BindExpression(syntax.Expression);
            return new BoundExpressionStatement(expression);
        }

        public BoundExpression BindExpression(ExpressionSyntax syntax)
        {
            switch (syntax.Kind)
            {
                case SyntaxKind.StringExpression:
                    return BindStringExpression((StringExpressionSyntax) syntax);
                case SyntaxKind.LiteralExpression:
                    return BindLiteralExpression((LiteralExpression)syntax);
                case SyntaxKind.UnaryExpression:
                    return BindUnaryExpression((UnaryExpression)syntax);
                case SyntaxKind.BinaryExpression:
                    return BindBinaryExpression((BinaryExpression)syntax);
                case SyntaxKind.ParenthesisExpression:
                    return BindParenthesisExpression((ParenthesisExpressionSyntax)syntax);
                case SyntaxKind.NameExpression:
                    return BindNameExpression((NameExpressionSyntax)syntax);
                case SyntaxKind.AssignmentExpression:
                    return BindAssignmentExpression((AssignmentExpression)syntax);
                case SyntaxKind.PrintExpression:
                    return BindPrintExpression((PrintExpression)syntax);
                case SyntaxKind.FunctionExpression:
                    return BindFunctionExpression((FunctionExpression)syntax);
                case SyntaxKind.DevelopFunctionExpression:
                    return BindDevelopFunctionExpression((DevelopFunctionExpression)syntax);
                case SyntaxKind.ReturnExpression:
                    return BindReturnExpression((ReturnExpression)syntax);
                default:
                    throw new Exception($"Unexpected syntax {syntax.Kind}");
            }
        }

        private BoundExpression BindReturnExpression(ReturnExpression syntax)
        {
            return new BoundLiteralExpression(syntax.ReturnKeyword);
        }

        private BoundExpression BindDevelopFunctionExpression(DevelopFunctionExpression syntax)
        {
            var name = syntax.IdentifierToken.Text;
            if (!_scope._functions.ContainsKey(name))
                _diagnostics.ReportUndefinedFunction(syntax.IdentifierToken.Span,name);
            return new BoundDevelopFunction(_scope._functions[name],_scope._variables[_scope._variableFunction[0]],BindExpression(syntax.Variable));
        }

        private BoundExpression BindFunctionExpression(FunctionExpression syntax)
        {
            var nameFunction = syntax.IdentifierToken.Text;
            var aux = new List<string>() { };
            aux.Add(syntax.Variable.Text);
            _scope.TryDeclare(new VariableSymbol(syntax.Variable.Text,typeof(int)));
            if(!_scope.TryDeclareFunction(aux,nameFunction,BindBlockStatement(syntax.Body)))
                _diagnostics.ReportRepeatedFunction(syntax.IdentifierToken.Span,nameFunction);
            return new BoundFunctionExpression(syntax.Body,syntax.Variable.Text);
        }

        private BoundExpression BindPrintExpression(PrintExpression syntax)
        {
            var aux = BindExpression(syntax.Expression);
            return new BoundPrintExpression(aux);
        }

        ///<summary>
        /// Bind the assignment expression
        ///</summary>
        private BoundExpression BindStringExpression(StringExpressionSyntax syntax)
        {
            var value = syntax.StringToken.Text ?? "";
            return new BoundLiteralExpression(value);
        }

        ///<summary>
        /// Bind the literal expression
        ///</summary>
        private BoundExpression BindParenthesisExpression(ParenthesisExpressionSyntax syntax)
        {
            return BindExpression(syntax.Expression);
        }
        ///<summary>
        /// Bind the literal expression
        ///</summary>
        private BoundExpression BindNameExpression(NameExpressionSyntax syntax)
        {
            var name = syntax.IdentifierToken.Text;
            if(!_scope.TryLookup(name,out var variable))
            {
                _diagnostics.ReportUndefinedName(syntax.IdentifierToken.Span, name);
                return new BoundLiteralExpression(0);
            }
            return new BoundVariableExpression(variable);
        }

        ///<summary>
        /// Bind the assignment expression
        ///</summary>
        private BoundExpression BindAssignmentExpression(AssignmentExpression syntax)
        {
            var name = syntax.IdentifierToken.Text;
            var boundExpression = BindExpression(syntax.Expression);
            if(!_scope.TryLookup(name, out var variable))
            {
                variable = new VariableSymbol(name, boundExpression.Type);
                _scope.TryDeclare(variable);
            }
            if(boundExpression.Type != variable.Type)
                _diagnostics.ReportCannotConvert(syntax.IdentifierToken.Span, variable.Type, boundExpression.Type);
            return new BoundAssignment(variable,boundExpression);     
        }
        ///<summary>
        /// Bind the literal expression
        ///</summary>
        private BoundExpression BindLiteralExpression(LiteralExpression syntax)
        {
            var value = syntax.Value ?? 0;
            return new BoundLiteralExpression(value);
        }

        ///<summary>
        /// Bind the unary expression
        ///</summary>

        private BoundExpression BindUnaryExpression(UnaryExpression syntax)
        {
            var boundOperand = BindExpression(syntax.Operand);
            var boundOperatorKind = BoundUnaryOperator.Bind(syntax.OperatorToken.Kind, boundOperand.Type);
            if(boundOperatorKind == null)
            {
                _diagnostics.ReportUndefinedUnaryOperator(syntax.OperatorToken.Span,syntax.OperatorToken.Text,boundOperand.Type);
                return boundOperand;
            }
            return new BoundUnaryExpression(boundOperatorKind,boundOperand);
        }
        
        ///<summary>
        /// Bind the binary expression
        ///</summary>
        private BoundExpression BindBinaryExpression(BinaryExpression syntax)
        {
            var boundLeft = BindExpression(syntax.Left);
            var boundRight = BindExpression(syntax.Right);
            var boundOperator = BoundBinaryOperator.BindFunction(syntax.OperatorToken.Kind);
            if(boundOperator == null)
            {
                _diagnostics.ReportUndefinedBinaryOperator(syntax.OperatorToken.Span,syntax.OperatorToken.Text,boundLeft.Type,boundRight.Type);
                return boundLeft;
            } 
            return new BoundBinaryExpression(boundLeft,boundOperator,boundRight);
        }
    }
}