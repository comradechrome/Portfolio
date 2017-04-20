package a06WordNet;

import edu.princeton.cs.introcs.StdOut;

public class Outcast {
	private final WordNet wordnet;

	public Outcast(WordNet wordnet) {         // constructor takes a WordNet object
		this.wordnet = wordnet;
	}
	public String outcast(String[] nouns) {  // given an array of WordNet nouns, return an outcast
		String winner = nouns[nouns.length - 1];								//last element as default
		int longest = 	wordnet.distance(nouns[nouns.length - 1], nouns[0]) 
						+ wordnet.distance(nouns[nouns.length - 1], nouns[nouns.length - 2]);

		int distance = 	wordnet.distance(nouns[0], nouns[1]) + wordnet.distance(nouns[0], nouns[nouns.length - 1]);
		if (distance > longest)  {
			longest = distance;
			winner = nouns [0];
		}
		for (int i = 1; i < nouns.length - 1; i++) {
			distance = wordnet.distance(nouns[i], nouns[i+1]) + wordnet.distance(nouns[i], nouns[i-1]);
			if (distance > longest) {
				longest = distance;
				winner = nouns[i];
			}
		}
		
		return winner;
	}
	public static void main(String[] args) {
		WordNet wordnet = new WordNet("/a06WordNet/synsets100.txt", "/a06WordNet/hypernyms100.txt");
	    Outcast outcast = new Outcast(wordnet);

        String[] nouns = {"Ig", "IgA", "jimdandy", "IgD", "IgE", "tetanus_immunoglobulin"};
        StdOut.println("Outcast: " + outcast.outcast(nouns));
	    
	}
}