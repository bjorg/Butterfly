//
// Sorting with Butterfly Networks
// (http://www.xilinx.com/labs/lava/sorter/sorter.htm)
// Steve G. Bjorg
// June 24, 2004
//

#include <iostream>


//
// Macros
//
#define FULLSIZE(size_log2) (1 << (size_log2))
#define HALFSIZE(size_log2) (1 << ((size_log2)-1))

//
// Comparison Function
//
inline void Compare(const int* i, int* o) {
   if(i[0] < i[1]) {
	   o[0] = i[0];
	   o[1] = i[1];
   } else {
	   o[1] = i[0];
	   o[0] = i[1];
   }
}


//
// Array Translation Classes & Function
//
template <int SIZE_LOG2>
class Identity {
public:
	int Size() const { return FULLSIZE(SIZE_LOG2); }
	int Translate(int i) const { return i; }
};

template <int SIZE_LOG2>
class Reverser {
public:
	int Size() const { return FULLSIZE(SIZE_LOG2); }
	int Translate(int i) const { return FULLSIZE(SIZE_LOG2) - 1 - i; }
};

template <int SIZE_LOG2>
class Riffle {
public:
	int Size() const { return FULLSIZE(SIZE_LOG2); }
	int Translate(int i) const { return (i >> 1) + ((i & 1) ? HALFSIZE(SIZE_LOG2) : 0); }
};

template <int SIZE_LOG2>
class Unriffle {
public:
	int Size() const { return FULLSIZE(SIZE_LOG2); }
	int Translate(int i) const {
		if(i >= HALFSIZE(SIZE_LOG2)) {
			return ((i - HALFSIZE(SIZE_LOG2)) << 1) + 1;
		} else {
			return i << 1;
		}
	}
};

template <class INDEXER>
inline void Copy(INDEXER indexer, const int* i, int* o) {
	for(int j = 0; j < indexer.Size(); ++j) {
		o[j] = i[indexer.Translate(j)];
	}
}


//
// Network Layout Classes
//
template <int SIZE_LOG2, class SORTER>
class Two {
public:
	void Sort(const int* i, int* o) {
		
		// sort lower half
		SORTER().Sort(i, o);
		
		// sort upper half
		SORTER().Sort(i + HALFSIZE(SIZE_LOG2), o + HALFSIZE(SIZE_LOG2));
	}
};

template <int SIZE_LOG2, class SORTER>
class Interleave {
public:
	void Sort(const int* i, int* o) {
		
		// unriffle inputs
		int t1[FULLSIZE(SIZE_LOG2)];
		Copy(Unriffle<SIZE_LOG2>(), i, t1);
		
		// apply sorter on upper and lower halves
		int t2[FULLSIZE(SIZE_LOG2)];
		Two<SIZE_LOG2, SORTER>().Sort(t1, t2);
		
		// riffle outputs
		Copy(Riffle<SIZE_LOG2>(), t2, o);
	}
};

template <int SIZE_LOG2>
class Evens {
public:
	void Sort(const int* i, int* o) {
		
		// loop over array and compare each entry pair
		for(int j = 0; j < FULLSIZE(SIZE_LOG2); j += 2) {
			Compare(i + j, o + j);
		}
	}
};

template <int SIZE_LOG2>
class Butterfly {
public:
	void Sort(const int* i, int* o) {
		int t[FULLSIZE(SIZE_LOG2)];
		
		// interleave and recursively apply the butterfly pattern
		Interleave<SIZE_LOG2, Butterfly<SIZE_LOG2-1> >().Sort(i, t);
		
		// apply pairwise comparisons
		Evens<SIZE_LOG2>().Sort(t, o);
	}
};

class Butterfly<1> {
public:
	void Sort(const int* i, int* o) {
		
		// base case of butterfly is simple comparison
		Compare(i, o);
	}
};

template <int SIZE_LOG2>
class Sorter {
public:
	void Sort(const int* i, int* o) {
		
		// apply sorter on upper and lower halves
		int t1[FULLSIZE(SIZE_LOG2)];
		Two<SIZE_LOG2, Sorter<SIZE_LOG2-1> >().Sort(i, t1);
		
		// copy lower half
		int t2[FULLSIZE(SIZE_LOG2)];
		Copy(Identity<SIZE_LOG2-1>(), t1, t2);
		
		// copy and reverse upper half
		Copy(Reverser<SIZE_LOG2-1>(), t1 + HALFSIZE(SIZE_LOG2), t2 + HALFSIZE(SIZE_LOG2));
		
		// apply butterfly merger
		Butterfly<SIZE_LOG2>().Sort(t2, o);
	}
};

class Sorter<1> {
public:
	void Sort(const int* i, int* o) {
		
		// degenerate case of sorter is simple comparison
		Compare(i, o);
	}
};

//template <class INDEXER>
//class Tester {
//public:
//	void Test(const char* name) {
//		std::cout << "Test " << name << "\n";
//		INDEXER indexer;
//		for(int i = 0; i < indexer.Size(); ++i) {
//			std::cout << (i + 1) << " -> " << (indexer.Translate(i) + 1) << "\n";
//		}
//		std::cout << "\n";
//	}
//};
//
//void TestReverser() {
//	Tester< Reverser<3> > tester;
//	tester.Test("Reverser");
//}
//
//void TestRiffle() {
//	Tester< Riffle<3> > tester;
//	tester.Test("Riffle");
//}
//
//void TestUnriffle() {
//	Tester< Unriffle<3> > tester;
//	tester.Test("Unriffle");
//}

void TestButterfly() {
	const int TEST_SIZE_LOG2 = 4;
	const int TEST_SIZE = FULLSIZE(TEST_SIZE_LOG2);
	
	// intialize array with numbers in reverse order
	int i[TEST_SIZE];
	for(int j = 0; j < TEST_SIZE; ++j) {
		i[j] = TEST_SIZE - 1 - j;
	}
	
	// sort using butterly network
	int o[TEST_SIZE];
	Butterfly<TEST_SIZE_LOG2>().Sort(i, o);
	
	// print result
	std::cout << "Sorted Result\n";
	for(int j = 0; j < TEST_SIZE; ++j) {
		std::cout << o[j] << "\n";
	}
}

int main (int argc, char * const argv[]) {
	TestButterfly();
    return 0;
}
