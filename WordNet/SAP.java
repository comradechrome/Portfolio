package a06WordNet;

import edu.princeton.cs.algs4.Bag;
import edu.princeton.cs.algs4.Digraph;
import edu.princeton.cs.algs4.DirectedCycle;
import edu.princeton.cs.introcs.In;
import edu.princeton.cs.introcs.StdOut;

public class SAP {
	private final Digraph G;	
	private DirectedCycle dC;
	
	/**
	 * constructor takes a digraph (not necessarily a DAG)
	 * @param G
	 */
	public SAP(Digraph G) {
		this.G = G;
		dC = new DirectedCycle(G);
	}

	/**
	 * is the digraph a directed acyclic graph?
	 * @return true if the digraph is a DAG
	 * @throws IllegalArgumentException if digraph has a cycle
	 */
	public boolean isDAG() {	
		if (dC.hasCycle()) { throw new IllegalArgumentException("Not a valid DAG"); }
		return true;
	}

	/**
	 * is the digraph a rooted DAG?
	 * @return false if not a valid DAG, otherwise true
	 */
	public boolean isRootedDAG() {
		if (!isDAG()) { return false; } // check if it is a valid DAG
		
		// check if there is one root
		int rootCount = 0;
		for (int vertex = 0; vertex < G.V(); vertex++) {
			if (G.outdegree(vertex) == 0) rootCount++;
		}
		
		if (rootCount != 1) return false;
		else return true;
	}

	/**
	 * length of shortest ancestral path between v and w; -1 if no such path
	 * @param v
	 * @param w
	 * @return
	 */
	public int length(int v, int w) {
		validateVertex(v);
		validateVertex(w);		
		int[] lengthResult = shortestPath(v, w);
		return lengthResult[0];
	}

	/**
	 * a common ancestor of v and w that participates in a shortest ancestral path; -1 if no such path
	 * @param v
	 * @param w
	 * @return
	 */
	public int ancestor(int v, int w) {
		validateVertex(v);
		validateVertex(w);
		int[] ancestorResult = shortestPath(v, w);
		return ancestorResult[1];
	}

	/**
	 * length of shortest ancestral path between any vertex in v and any vertex in w; -1 if no such path
	 * @param v
	 * @param w
	 * @return shortest path or -1 if no path exists
	 */
	public int length(Iterable<Integer> v, Iterable<Integer> w) {
		validateVertex(v);
		validateVertex(w);
		int[] lengthResult = shortest(v, w);
		return lengthResult[0];
	}
 
	/**
	 * a common ancestor that participates in shortest ancestral path; -1 if no such path
	 * @param v
	 * @param w
	 * @return common ancestor or -1 if no path exists
	 */
	public int ancestor(Iterable<Integer> v, Iterable<Integer> w) {
		validateVertex(v);
		validateVertex(w);
		int[] ancestorResult = shortest(v, w);
		return ancestorResult[1];
	}
	
	/**
	 * Private helper method to validate vertex
	 * @param vertex
	 * @throws IndexOutOfBoundsException if vertex is less than 0 or greater than the number of vertices in the digraph
	 */
	private void validateVertex(int vertex) {
		if (vertex < 0 || vertex >= G.V()) throw new IndexOutOfBoundsException();
	}
	
	/**
	 * Private helper method to validate vertex
	 * @param vertex
	 * @throws IndexOutOfBoundsException if vertex is less than 0 or greater than the number of vertices in the digraph
	 */
	private void validateVertex(Iterable<Integer> vertex) {
		for (Integer v : vertex) { if (v < 0 || v >= G.V()) throw new IndexOutOfBoundsException(); }
	}
	
	/**
	 * Private helper method for length(int v, int w) & ancestor(int v, w)
	 * @param v
	 * @param w
	 * @return -1 if no path exists, otherwise return the shortest path ([0]) / common ancestor ([1])
	 */
	private int[] shortestPath(int v, int w) {
		int[] result = new int[2];  
        DeluxeBFS vDBFS = new DeluxeBFS(G, v);  
        DeluxeBFS wDBFS = new DeluxeBFS(G, w);  
        boolean[] vMarked = vDBFS.getMarked();  
        boolean[] wMarked = wDBFS.getMarked();  
        int shortestLength = Integer.MAX_VALUE;  
        int tempLength = Integer.MAX_VALUE;  
        int shortestAncestor = Integer.MAX_VALUE;  
        
        for (int i=0; i < vMarked.length; i++){  
            if (vMarked[i] && wMarked[i]){  
                tempLength = vDBFS.distTo(i) + wDBFS.distTo(i);  
                if (tempLength < shortestLength) {  
                    shortestLength = tempLength;  
                    shortestAncestor = i;  
                }  
            }  
        }  
        
        if (shortestLength == Integer.MAX_VALUE){  
            result[0] = -1;  
            result[1] = -1;  
            return result;  
        }  
        
        result[0] = shortestLength;  
        result[1] = shortestAncestor;  
        return result;
	}
	
	/**
	 * Private helper method for length(Iterable<Integer> v, Iterable<Integer> w) & ancestor(Iterable<Integer> v, Iterable<Integer> w)
	 * @param v
	 * @param w
	 * @return -1 if no path exists, otherwise return the shortest path ([0]) / common ancestor
	 */
	private int[] shortest(Iterable<Integer> v, Iterable<Integer> w) {
		int shortestAncestor = Integer.MAX_VALUE;  
        int shortestLength = Integer.MAX_VALUE;  
        int[] result = new int[2];  
        
        for (int vNode : v){  
            for (int wNode : w){  
                int[] tempResult = shortestPath(vNode, wNode);  
                if (tempResult[0] != -1 && tempResult[0] < shortestLength){  
                    shortestLength = tempResult[0];  
                    shortestAncestor = tempResult[1];  
                }  
            }  
        }  
        
        if (shortestLength == Integer.MAX_VALUE){  
            result[0] = -1;  
            result[1] = -1;  
            return result;  
        }  
        
        result[0] = shortestLength;  
        result[1] = shortestAncestor;  
        return result;
	}
	

	// do unit testing of this class 
	public static void main(String[] args) {
		In in = new In("/a06WordNet/digraph1.txt");
	    Digraph G = new Digraph(in);
	    StdOut.println(G);
//	    G.addEdge(1, 6);
	    SAP sap = new SAP(G);
	    
	    StdOut.println("acyclic? " + sap.isDAG());
	    StdOut.println("rooted? " + sap.isRootedDAG());
	    in = new In("/a06WordNet/digraph1.txt");
	    in.readInt();
	    in.readInt();
	    
	    int length;
	    int ancestor;
	    while (!in.isEmpty()) {
	        int v = in.readInt();
	        int w = in.readInt();
//	        StdOut.println(v + " " + w);
	        
	        length   = sap.length(v, w);
	        ancestor = sap.ancestor(v, w);
//	        StdOut.printf("length = %d, ancestor = %d\n", length, ancestor); 
	    }
	    
	    length   = sap.length(11, 6);
        ancestor = sap.ancestor(11, 6);
	    StdOut.printf("length = %d, ancestor = %d\n", length, ancestor); 
	    
	    Bag<Integer> bag1 = new Bag<>();
	    Bag<Integer> bag2 = new Bag<>();
	    bag1.add(11);
	    bag1.add(2);
	    bag2.add(1);
	    
	    length   = sap.length(bag1, bag2);
        ancestor = sap.ancestor(bag1, bag2);
	    StdOut.printf("length = %d, ancestor = %d\n", length, ancestor); 
	}
}