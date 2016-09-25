using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tester
{
	using System.Runtime.InteropServices;
	using OdoyuleRules;
	using OdoyuleRules.Designer;
	using OdoyuleRules.Visualization;

	class Program
	{
		static void Main(string[] args)
		{
			var protocol1 = new Protocol() { Kv = 200, Ma = 200, Name = "Cardiac" };
			var protocol2 = new Protocol() { Kv = 20, Ma = 200, Name = "Scout" };

			Console.WriteLine("P1: " + protocol1);
			Console.WriteLine("P2: " + protocol2);

			var engine = RulesEngineFactory.New(x =>
			{
				x.Rules(r =>
				{
					r.Add<Temp>();
					r.Add<Temp2>();
					//r.Add<MaxKvRule>();
					//r.Add<MinKvRule>();
					//r.Add<ScoutKvRule>();
				});
			});

			var visitor = new TextRuntimeVisitor();
			engine.Accept(visitor);
			Console.WriteLine(visitor);

			using (var session = engine.CreateSession())
			{
				session.Add(protocol1);
				session.Add(protocol2);
				
				session.Run();

				var changes = session.Facts<Changed>().ToArray();

				Console.WriteLine("\n\n");

				foreach (var change in changes)
				{
					Console.WriteLine("{0} has changed in {1}", change.Fact.Parameter, change.Fact.Name);
				}
			}
		

			Console.WriteLine("P1: " + protocol1);
			Console.WriteLine("P2: " + protocol2);

			Console.ReadLine();
		}
	}

	class Temp : RuleDefinition
	{
		public Temp()
		{
			Fact<Protocol>()
				.When(p => p.Name.Length != 5 )
				.Then(x => x.Delegate(p => Console.WriteLine("{0} name length != 5", p.Name)));
		}
	}

	class Temp2 : RuleDefinition
	{
		public Temp2()
		{
			Fact<Protocol>()
				.When(p => p.Name.Length == 5)
				.Then(x => x.Delegate(p => Console.WriteLine("{0} name length == 5", p.Name)));
		}
	}

	class MaxKvRule : RuleDefinition
	{
		public MaxKvRule()
		{
			Fact<Protocol>()
				.When(p => p.Kv > 140)
				.Add(p => new Changed() { Name = p.Name, Parameter = "Kv" })
				.Then(x => x.Delegate(p => p.Kv = 140));
		}
	}

	class MinKvRule : RuleDefinition
	{
		public MinKvRule()
		{
			Fact<Protocol>()
				.When(p => p.Kv < 50)
				.Add(p => new Changed() { Name = p.Name, Parameter = "Kv" })
				.Then(x => x.Delegate(p => p.Kv = 50));
		}
	}

	class ScoutKvRule : RuleDefinition
	{
		public ScoutKvRule()
		{
			Fact<Protocol>()
				.When(p => p.Name == "Scout")
				.When(p => p.Kv >= 80)
				.Add(p => new Changed() {Name = p.Name, Parameter = "Kv"})
				.Then(x => x.Delegate(p => p.Kv = 80));
		} 
	}


	public class Protocol
	{
		public int Kv { get; set; }
		public int Ma { get; set; }
		public string Name { get; set; }

		public override string ToString()
		{
			return string.Format("Protocol: Name = {0}, kV = {1}, Ma = {2}", Name, Kv, Ma);
		}
	}

	class Changed
	{
		public string Name { get; set; }
		public string Parameter { get; set; }
	}
}
