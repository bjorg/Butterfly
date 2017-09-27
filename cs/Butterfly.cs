using System;
using System.Diagnostics;
using Hardware;
namespace Networks {

	public class OddSizedSignalException : System.ApplicationException {
		public OddSizedSignalException (string message)  : base (message) {}
	}

	public class ZipException : System.ApplicationException {
		public ZipException (string message)  : base (message) {}
	}
	

	public class need_two_element_list : System.ApplicationException {
		public need_two_element_list (string message)  : base (message) {}
	}

	/// <summary>
	/// This class defines a signal type that represents values over wires.
	/// </summary>
	/// 
	

	public class Butterfly {

		public static SignalList zip (SignalPair isl) {
			SignalList fl = (SignalList) isl.first ;
			SignalList sl = (SignalList) isl.second ;
			if (fl.val.Length != sl.val.Length) {
				ZipException ZipProblem = new ZipException ("Zip needs equal length lists.") ;
				throw ZipProblem ;
			}
			SignalPair [] spl = new SignalPair[fl.val.Length]  ;
			for (int i = 0 ; i < fl.val.Length; i++)
				spl[i] = new SignalPair (fl.val[i], sl.val[i]) ;
			SignalList result = new SignalList (spl) ;
			return result ;
		}

		public static SignalPair unzip(SignalList isl) {
			Signal[] val = isl.val ;
			Signal[] fl = new Signal [val.Length] ;
			Signal[] sl = new Signal [val.Length] ;
			for (int i=0; i < val.Length; i++) {
				fl[i] = ((SignalPair)val[i]).first ;
				sl[i] = ((SignalPair)val[i]).second ;
			}
			SignalPair result = new SignalPair (new SignalList (fl), new SignalList (sl)) ;
			return result ;
		}


		public static SignalPair halve (SignalList isl) {
			if (isl.Length () % 2 == 1) {
				OddSizedSignalException OddSizedSignal = new OddSizedSignalException ("Odd sized signal list.") ;
				throw OddSizedSignal ;
			}
			Signal[] val = isl.val ;
			int half_length = val.Length/2 ;
			Signal[] fl = new Signal[half_length] ;
			Signal[] sl = new Signal[half_length] ;
			for (int i=0; i < half_length; i++) { 
				fl[i] = val[i] ;
				sl[i] = val[half_length+i] ;
			}
			return new SignalPair (new SignalList (fl), new SignalList (sl)) ;
		}

		public static SignalList pair (SignalList sl) {
			Signal[] rsl = new Signal[sl.Length()/2] ;
			for (int i=0; i<sl.Length()/2; i++)
				rsl[i] = new SignalPair (sl.val[2*i], sl.val[2*i+1]) ;
			return new SignalList (rsl) ;
		}

		public static SignalList chop2 (SignalList sl) {
			Signal[] rsl = new Signal[sl.Length()/2] ;
			for (int i=0; i<sl.Length()/2; i++)
				rsl[i] = new SignalList (sl.val[2*i], sl.val[2*i+1]) ;
			return new SignalList (rsl) ;
		}

		public static SignalList unpair (SignalList sl) {
			Signal[] rsl = new Signal[2*sl.Length()] ;
			for (int i=0; i<sl.Length(); i++) {
				rsl[2*i] = ((SignalPair)sl.val[i]).first ;
				rsl[2*i+1] = ((SignalPair)sl.val[i]).second ;
			}
			return new SignalList (rsl) ;
		}

		public static SignalList unhalve (SignalPair sp) {
			SignalList fl = (SignalList)sp.first ;
			SignalList sl = (SignalList)sp.second ;
			SignalList s = new SignalList (fl, sl) ;
			return concat (s) ;
		}

		public static SignalList concat (SignalList isl) {
			Signal[] val = isl.val ;
			int len = 0 ;
			foreach (SignalList v in val) {
				len += v.val.Length ;
			}
			Signal[] sl = new Signal[len] ;
			int i = 0 ;
			foreach (Signal vl in val)
				foreach (Signal v in ((SignalList)vl).val) {
					sl[i] = v ;
					i++ ;
				}
			SignalList result = new SignalList (sl) ;
			return result ;
		}

		public static SignalList reverse (SignalList sl) {
			Signal[] vl = new Signal[sl.Length()] ;
			for (int i = 0; i < sl.Length(); i++)
				vl[i] = sl.val[i] ;
			Array.Reverse (vl) ;
			return new SignalList (vl) ;
		}

		public static SignalList riffle (SignalList sl) { 
			return Butterfly.unpair (Butterfly.zip (Butterfly.halve (sl))) ;
		}

		
		public static SignalList unriffle (SignalList sl) {
			return Butterfly.unhalve (Butterfly.unzip (Butterfly.pair (sl))) ;
		}

		
		public static SignalPair par2 (SignalListToSignalList f1, 
				                       SignalListToSignalList f2, 
				                       SignalPair input) {
			return new SignalPair (f1 ((SignalList)input.first), f2 ((SignalList)input.second));
		}


		public static SignalList two (SignalListToSignalList r, SignalList l) {
			return Butterfly.unhalve (Butterfly.par2 (r, r, Butterfly.halve (l)) ) ;
		}

		public static SignalList map (SignalListToSignalList f, SignalList l) {
			SignalList[] rl = new SignalList[l.Length()] ;
			for (int i=0; i<l.Length(); i++) {
				rl[i] = f ((SignalList)(l.val[i])) ;
			}
			return new SignalList (rl) ;
		}

		public static SignalList evens (SignalListToSignalList f, SignalList l) {
			return Butterfly.concat (Butterfly.map (f, Butterfly.chop2 (l))) ;
		}

		public static SignalList ilv (SignalListToSignalList f, SignalList l) {
			return Butterfly.riffle (Butterfly.two (f, Butterfly.unriffle (l))) ;
		}


			public static SignalList two_sorter (SignalList sl) {
				if (sl.Length () != 2) {
					need_two_element_list exeception = new need_two_element_list ("Odd sized signal list.") ;
					throw exeception ;
				}
				if (((SignalInt)sl.val[0]).val < ((SignalInt)sl.val[1]).val)
					return new SignalList (sl.val[0], sl.val[1]) ;
				else
					return new SignalList (sl.val[1], sl.val[0]) ;
			}


		public static SignalList merge (SignalList l) {
			if (l.Length() == 2)
				return Butterfly.two_sorter (l) ;
			else {
				SignalListToSignalList bfly = new SignalListToSignalList (Butterfly.merge) ;
				SignalListToSignalList sort2 = new SignalListToSignalList (Butterfly.two_sorter) ;
				return Butterfly.evens (sort2, Butterfly.ilv (bfly, l)) ;
			}
		}


		public static SignalList sorter (SignalList l){
			if (l.Length() == 2)
				return Butterfly.two_sorter (l) ;
			else {
				SignalListToSignalList sort = new SignalListToSignalList (Butterfly.sorter) ;
				SignalList subsorts = Butterfly.two (sort, l) ;
				SignalPair hl = Butterfly.halve (subsorts) ;
				SignalPair revtop = new SignalPair (hl.first, Butterfly.reverse ((SignalList)(hl.second))) ;
				SignalList submerge = Butterfly.unhalve (revtop) ;
				return Butterfly.merge (submerge) ;
			}
		}

	}

}



