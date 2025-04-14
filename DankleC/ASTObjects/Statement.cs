using DankleC.ASTObjects.Expressions;
using DankleC.IR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DankleC.ASTObjects
{
	public abstract class Statement : IASTObject
	{
		public int ID { get; } = IDRandomizer.Next();

#pragma warning disable CS8618
		public IRScope Scope { get; internal set; }
#pragma warning restore CS8618

		public abstract void BuildIR(IRBuilder builder, IRFunction func);

		public static readonly Random IDRandomizer = new();
	}

	public class ReturnStatement(IExpression? expression) : Statement
	{
		public readonly IExpression? Expression = expression;

		public override void BuildIR(IRBuilder builder, IRFunction func)
		{
			var expr = Expression?.Resolve(builder);
			var value = expr?.Cast(func.Type.ReturnType).Execute(builder);

			if (value is not null) builder.Add(new IRSetReturn(value));
			else if (func.Type.ReturnType != new BuiltinTypeSpecifier(BuiltinType.Void)) throw new InvalidOperationException($"Function \"{func.Name}\" must return a value");
			builder.Add(new EndFrame());
			builder.Add(new IRReturnFunc());
		}
	}

	public class InitAssignmentStatement(TypeSpecifier type, string name, IExpression expr) : Statement
	{
		public readonly TypeSpecifier Type = type;
		public readonly string Name = name;
		public readonly IExpression Expression = expr;

		public override void BuildIR(IRBuilder builder, IRFunction func)
		{
			if (Type.IsStatic)
			{
				if (Expression is not IToBytes i) throw new InvalidOperationException("Static variable must have constant expression");
				Scope.AllocStaticLocal(Name, Type, i.ToBytes(builder));
				return;
			}

			var value = Expression.Resolve(builder).Cast(Type).Execute(builder);
			var variable = Scope.AllocLocal(Name, Type);
			variable.Store(builder, value);
		}
	}
	
	public class DeclareStatement(TypeSpecifier type, string name) : Statement
	{
		public readonly TypeSpecifier Type = type;
		public readonly string Name = name;

		public override void BuildIR(IRBuilder builder, IRFunction func)
		{
			if (Type.IsStatic) Scope.AllocStaticLocal(Name, Type, new Bytes(new byte[Type.Size]));
			else Scope.AllocLocal(Name, Type);
		}
	}

	public class LabelStatement(string label) : Statement
	{
		public readonly string Label = label;

        public override void BuildIR(IRBuilder builder, IRFunction func)
        {
			builder.Add(new IRLabel($"{func.SymbolName}${Label}"));
        }
    }

	public class GotoStatement(string label) : Statement
	{
		public readonly string Label = label;

        public override void BuildIR(IRBuilder builder, IRFunction func)
        {
			builder.Add(new IRJump(new IRLabel($"{func.SymbolName}${Label}")));
        }
    }

    public class EmptyStatement : Statement
	{
		public override void BuildIR(IRBuilder builder, IRFunction func)
		{

		}
	}
}
