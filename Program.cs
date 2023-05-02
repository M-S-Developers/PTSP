/*
 * Created by SharpDevelop.
 * User: mspma
 * Date: 5/2/2023
 * Time: 3:51 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using System.Collections.Generic;

namespace PTSP
{
	public class State : IComparable<State>, IEqualityComparer<State> {
		public ArrayList path = new ArrayList();
		
		public HashSet<int> cities = new HashSet<int>();

		public static HashSet<State> states = new HashSet<State>();

		public State() {
		}
				
		public int CompareTo(State s) {
			if (this.path.Count != s.path.Count) {
				return Math.Sign(this.path.Count - s.path.Count);
			}
			
			for (int i = 0; i < this.path.Count; ++i) {
				if ((int) this.path[i] != (int) s.path[i]) {
					return Math.Sign((int) this.path[i] - (int) s.path[i]);
				}
			}
			
			return 0;
		}
		
		public State AddCity(int position, int city) {
			State s = new State();
			
			for (int i = 0; i < position; ++i) {
				s.path.Add(this.path[i]);
				s.cities.Add((int) this.path[i]);
			}
			
			s.path.Add(city);
			s.cities.Add((int) city);
			
			for (int i = position; i < this.path.Count; ++i) {
				s.path.Add(this.path[i]);
				s.cities.Add((int) this.path[i]);
			}
			
			if (states.Contains(s)) {
				return s;
			}
			
			states.Add(s);
			
			return s;
		}
		
		public int Length() {
			int l = 0;
			
			for (int i = 1; i < this.path.Count; ++i) {
				l += Program.M[(int) this.path[i - 1], (int) this.path[i]];
			}
			
			return l;
		}
		
		public String ToString() {
			string result = "";
			
			for (int i = 0; i < this.path.Count; ++i) {
				if (i > 0) {
					result += " ";
				}
				
				result += this.path[i];
			}
			
			return result;
		}
		
		public bool Equals(State a, State b) {
			return a.CompareTo(b) == 0;
		}
	 	
		public int GetHashCode(State state) {
			return this.path.GetHashCode();
		}
	}
	
	public class StateQueueComparer : IComparer<StateHolder> {
		public int Compare(StateHolder a, StateHolder b) {
			return Math.Sign(a.state.Length() - b.state.Length());
		}
	}
	
	public class StateHolder : IComparable<StateHolder> {
		public State state;
		
		public StateHolder(State state) {
			this.state = state;
		}
		
		public int CompareTo(StateHolder stateHolder) {
			return this.state.Length() - stateHolder.state.Length();
		}
	}
	
	class Program
	{
		public static int[,] M;
		
		public static bool[] V;
		
		public static int[] O;
		
		public static void Optimal(int n) {
			V = new bool[n];
			
			int[] path = new int[n];
			
			for (int i = 0; i < n; ++i) {
				V[i] = false;
				
				path[i] = -1;
			}
			
			Step(n, 0, -1, path);
			
			int l = 0;
			
			for (int i = 1; i < path.Length; ++i) {
				l += M[O[i - 1], O[i]];
			}
			
			Console.WriteLine("-- Optimal --");
			
			Console.WriteLine(n + " " + l);
			
			for (int i = 0; i < path.Length; ++i) {
				if (i > 0) {
					Console.Write(" ");
				}
				
				Console.Write(O[i]);
			}
			
			Console.WriteLine();
		}
		
		public static void Step(int n, int step, int city, int[] path) {
			if (step == n) {
				if (O == null) {
					O = new int[n];
					
					for (int i = 0; i < step; ++i) {
						O[i] = path[i];
					}
				} else {
					int l = 0, lc = 0;
					
					for (int i = 1; i < step; ++i) {
						l += M[O[i - 1], O[i]];
						
						lc += M[path[i - 1], path[i]];
					}
					
					if (lc < l) {
						for (int i = 0; i < step; ++i) {
							O[i] = path[i];
						}
					}
				}
				
				return;
			}
			
			for (int i = 0; i < n; ++i) {
				if (!V[i]) {
					V[i] = true;
					
					path[step] = i;
					
					Step (n, step + 1, i, path);
					
					V[i] = false;
				}
			}
		}
		
		public static void Main(string[] args)
		{
			Console.WriteLine("PTSP - Copyright Mirzakhmet Syzdykov 2023 (c)");
			
			Console.WriteLine("");

			Random rnd = new Random(DateTime.Now.Millisecond);
			
			int n = rnd.Next(100, 120);
			
			M = new int[n, n];
			
			for (int i = 0; i < n; ++i) {
				for (int j = 0; j < n; ++j) {
					if (rnd.Next(0, 100) >= 6) {
						M[i, j] = 1;
					} else {
						M[i, j] = 0;
					}
				}
				
				M[i, i] = -1;
			}
			
			for (int i = 0; i < n; ++i) {
				for (int j = 0; j < n; ++j) {
					Console.Write(" " + M[i, j]);
				}
				
				Console.WriteLine();
			}
			
			State start = new State();
						
			State answer = null;
			
			int tries = 1000000000, currentTries = 0;
			
			HashSet<State> passedStates = new HashSet<State>();
			
			SortedSet<StateHolder> priorityQueue = new SortedSet<StateHolder>(new StateQueueComparer());
			
			priorityQueue.Add(new StateHolder(start));
			
			while (++currentTries <= tries) {
				State minimal = null;				

				ArrayList list = new ArrayList();
				
				foreach (StateHolder stateHolder in priorityQueue) {
					if (passedStates.Contains(stateHolder.state)) {
						continue;
					}
					
					list.Add(stateHolder.state);
					
					if (minimal == null || minimal.Length() > stateHolder.state.Length()) {
						minimal = stateHolder.state;
						
						list.Remove(stateHolder.state);
						
						passedStates.Add(stateHolder.state);
						
						break;
					}
				}
				
				priorityQueue.Clear();
				
				foreach (State state in list) {
					priorityQueue.Add(new StateHolder(state));
				}
								
				if (minimal == null) {
					foreach (State state in passedStates) {
						if (state.cities.Count == n) {
							if (answer == null || (state != null && answer.Length() > state.Length())) {
								answer = state;
							}
						}
					}
					
					break;
				}
								
				for (int i = 0; i < n; ++i) {
					if (!minimal.cities.Contains(i)) {
						int k = minimal.path.Count;
						
						for (int j = 0; j <= k; ++j) {
							State t = minimal.AddCity(j, i);
							if (!passedStates.Contains(t)) {
								priorityQueue.Add(new StateHolder(t));								
							}
						}
					}
				}

				System.GC.Collect();
			}
			
			if (answer != null) {
				Console.WriteLine(answer.cities.Count + " " + answer.Length());
				
				Console.WriteLine(answer.ToString());
			}
			
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
		}
	}
}