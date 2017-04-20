package a06WordNet;

import edu.princeton.cs.algs4.Bag;
import edu.princeton.cs.algs4.Digraph;
import edu.princeton.cs.algs4.ST;
import edu.princeton.cs.introcs.In;
import edu.princeton.cs.introcs.StdOut;

public class WordNet {
	private final SAP sap;
	private Digraph G;
	private final ST<Integer, String> synsetTable;
	private final ST<String, Bag<Integer>> nounInstanceTable;
	private String[] elements;
	private String[] nouns;
	private int synsetId;
	private int hypernymId;
	
	// constructor takes the name of the two input files
	public WordNet(String synsets, String hypernyms) {
		if (synsets == null || hypernyms == null) throw new NullPointerException(); 
		
		synsetTable = new ST<>();
		nounInstanceTable = new ST<>();
		
		readSynsets(synsets);
		sap = new SAP(readHypernyms(hypernyms)); 
		sap.isDAG();
	}
	
	/**
	 * Read in synsets
	 * @param synsetFile
	 */
	private void readSynsets(String synsetFile) {
		In in = new In(synsetFile);
		Bag<Integer> bag;
		
		while(in.hasNextLine()) {
			elements = in.readLine().split(","); 
			synsetId = Integer.parseInt(elements[0]); 
			synsetTable.put(synsetId, elements[1]);
			
			nouns = elements[1].split(" ");
			for(String noun : nouns) {
				bag = nounInstanceTable.get(noun);
				
				if (bag == null) {
					bag = new Bag<>();
					bag.add(synsetId);
					nounInstanceTable.put(noun, bag);
				} else { 
					bag.add(synsetId); }
			}
		}
	}

	/**
	 * Read in hypernyms
	 * @param hypernymsFile
	 * @return 
	 */
	private Digraph readHypernyms(String hypernymsFile) {
		G = new Digraph(synsetTable.size());
		In in = new In(hypernymsFile);
		
		while(in.hasNextLine()) {
			elements = in.readLine().split(",");
			hypernymId = Integer.parseInt(elements[0]);
			if (elements.length != 1)
				for (int i = 1; i < elements.length; i++) {
					G.addEdge(hypernymId, Integer.parseInt(elements[1]));
				}
		}
		return G;
	}

	// returns all WordNet nouns
	public Iterable<String> nouns() {
		return nounInstanceTable.keys();
	}

	// is the word a WordNet noun?
	public boolean isNoun(String word) {
		if (word == null) throw new NullPointerException("The word is null");
			return nounInstanceTable.contains(word);
	}

	// distance between nounA and nounB (defined below)
	public int distance(String nounA, String nounB) {
		if (!isNoun(nounA) || !isNoun(nounB)) throw new NullPointerException("nounA or nounB is not in WordNet");
		return sap.length(nounInstanceTable.get(nounA), nounInstanceTable.get(nounB));
	}

	// a synset (second field of synsets.txt) that is the common ancestor of nounA and nounB
	// in a shortest ancestral path (defined below)
	public String sap(String nounA, String nounB) {
		if (nounA == null || nounB == null) throw new NullPointerException("nounA or nounB is null");
		return synsetTable.get(sap.ancestor(nounInstanceTable.get(nounA), nounInstanceTable.get(nounB)));
	}

	// do unit testing of this class
	public static void main(String[] args) {
		WordNet wn = new WordNet("/a06WordNet/synsets.txt", "/a06WordNet/hypernyms.txt");
//		StdOut.print(wn.nouns());
		StdOut.println(wn.isNoun("Hemofil"));
		StdOut.println(wn.distance("Hemofil", "Ig"));
		StdOut.println(wn.sap("jimdandy", "IgE"));
	}
}