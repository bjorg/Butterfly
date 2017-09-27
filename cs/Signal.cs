using System;
using System.Diagnostics;

namespace Hardware {

	abstract public class Signal {
		abstract public void Write () ;
		public void WriteLine () { 
			this.Write () ; 
			Debug.WriteLine ("") ; 
		}
	}

	public delegate Signal SignalToSignal (Signal s) ;


	public class SignalInt : Signal {
		public int val ;

		public SignalInt (int v) { val = v ; }

		public override void  Write () { Debug.Write (val) ; }
	}


	public class SignalList : Signal {
		public Signal[] val ;

		// Reuse supplied signal list upon construction.
		public SignalList (params Signal[] v) { val = v ; }

		// Create a new signal list from an int array upon construction.
		public SignalList (params int[] v) {
			val = new Signal [v.Length] ;
			for (int i = 0; i < v.Length; i++) {
				val[i] = new SignalInt (v[i]) ;
			}
		}

		public int Length () {
			return val.Length ;
		}

		public override void  Write () {
			Debug.Write ("[") ;
			for(int i  = 0 ; i < val.Length; i++) {
				val[i].Write () ;
				if (i < val.GetUpperBound(0))
					Debug.Write (", ") ;
			}
			Debug.Write ("]") ;
		}

	}

	public delegate SignalList SignalListToSignalList (SignalList s) ;


	public class SignalPair : Signal {
		public Signal first ;
		public Signal second ;
		public SignalPair (Signal f, Signal s) { first = f ; second = s ; }

		public override void Write () {
			Debug.Write ("(") ;
			first.Write () ;
			Debug.Write (", ") ;
			second.Write () ;
			Debug.Write (")") ;
		}

	}

	public delegate SignalPair SignalPairToSignalPair (SignalPair s) ;
	public delegate SignalList SignalPairToSignalList (SignalPair s) ;
	public delegate SignalPair SignalListToSignalPair (SignalPair s) ;
}
