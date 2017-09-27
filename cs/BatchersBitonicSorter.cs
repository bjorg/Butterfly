using System;
using System.Diagnostics;
using Hardware;
using Networks;
using Win32;

namespace BatchersBitonicSorter
{




	class TestBatchersSorter
	{		


		public SignalList two_sorter (SignalList sl) {
			if (sl.Length () != 2) {
			need_two_element_list exeception = new need_two_element_list ("Odd sized signal list.") ;
			  throw exeception ;
			}
			if (((SignalInt)sl.val[0]).val < ((SignalInt)sl.val[1]).val)
				return new SignalList (sl.val[0], sl.val[1]) ;
			else
				return new SignalList (sl.val[1], sl.val[0]) ;
		}


		static void DebugMain()
		{
			Debug.WriteLine ("Batchers Bitonic Sorter.") ;

			int[] s1v = new int[] {1,2,3,4,5,6,7,8} ;
			SignalList s1 = new SignalList (s1v) ;
			s1.WriteLine () ; 

			SignalPair sp = Butterfly.halve (s1) ;
			sp.WriteLine () ; 

			SignalList zl = Butterfly.zip (sp) ;
			zl.WriteLine () ;

			SignalPair uz = Butterfly.unzip(zl) ;
			uz.WriteLine () ;

			SignalList s1_pair = Butterfly.pair (s1) ;
			s1_pair.WriteLine () ;

			SignalList up = Butterfly.unpair(s1_pair) ;
			up.WriteLine ();

			int[] s2v = new int[] {9,10,11} ;
			SignalList s2 = new SignalList (s2v) ;

			int[] s3v = new int[] {12,13,14,15} ;
			SignalList s3 = new SignalList (s3v) ;

			SignalList[] s4a = {s1, s2, s3} ;
			SignalList s4 = new SignalList (s4a) ;

			s4.WriteLine() ;

			SignalList s5 = Butterfly.concat(s4) ;
			s5.WriteLine () ;

			SignalList rl = Butterfly.riffle(s1) ;
			rl.WriteLine () ;

			SignalList url = Butterfly.unriffle(rl) ;
			url.WriteLine () ;

			SignalListToSignalList reverse = new SignalListToSignalList (Butterfly.reverse) ;
			SignalListToSignalList riffle = new SignalListToSignalList (Butterfly.riffle) ;
			SignalPair fs_par2 = Butterfly.par2 (reverse, riffle, sp) ;
			fs_par2.WriteLine () ;

			SignalList two_reverse = Butterfly.two (reverse, s1) ;
			two_reverse.WriteLine() ;

			SignalList s6 = new SignalList (new SignalInt (8), new SignalInt (4)) ;
			s6.WriteLine () ;

			SignalList s6_sorted = Butterfly.two_sorter (s6) ;
			s6_sorted.WriteLine() ;

			SignalList s7 = new SignalList (new SignalInt (8), new SignalInt (4), new SignalInt (3), new SignalInt (7)) ;
			SignalListToSignalList sort2 = new SignalListToSignalList (Butterfly.two_sorter) ;
			SignalList s7_evens = Butterfly.evens (sort2, s7) ;
			s7_evens.WriteLine () ;
			
			SignalList s7_merged = Butterfly.merge (s7) ;
			s7_merged.WriteLine () ;

			SignalList s8 = new SignalList (1, 2, 3, 4, 8, 7, 6, 5) ;
			(Butterfly.merge (s8)).WriteLine () ;

			SignalList s9 = new SignalList (8, 7, 6, 5, 4, 3, 2, 1) ;
			(Butterfly.sorter (s9)).WriteLine () ;

		}


		/// <summary>
		/// The main entry point for the Batcher's Bitonic Sorter application.
		/// </summary>
		/// 
		[STAThread]
		static void Main(string[] args) {
            HiPerfTimer timer = new HiPerfTimer();
			int[] sa = new int [256] ;
			for (int i=0; i<256; i++)
				sa[i] = 255-i ;
			SignalList sl = new SignalList (sa) ;
            timer.Start();
            SignalList sorted_sl = Butterfly.sorter (sl) ;
            timer.Stop();
			sorted_sl.WriteLine () ;
			Console.Write ("sort 256 duration: ") ;
			Console.WriteLine (timer.TotalDuration) ;

		}

	}

}
