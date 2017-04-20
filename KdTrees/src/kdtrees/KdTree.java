package kdtrees;

import java.util.Comparator;
import java.util.List;

import edu.princeton.cs.algs4.Stack;

public class KdTree<Key extends Comparable<Key>, Value> {
	
	private List<Comparator<Key>> comps;
	private static int size = 0;
	
	/**
	 * Create a KD Tree with K comparators
	 * @param comps[] depth Comparators in order of evaluation
	 */
	public KdTree(List<Comparator<Key>> comps){
		this.comps = comps;
	}

	private Node root;
	
	private class Node{
		private Key key;
		private Value val;
		private Node less, more;
		private int depth;
		
		public Node(Key key, Value val, int depth){
			size++;//static counter of all nodes in existence
			this.key = key;
			this.val = val;
			this.depth = depth;
		}
	}
	
	public int size(){
		return size;
	}
	
	public Boolean contains(Key key){
		return null != get(key);
	}
	
	public Value get(Key key){
		return get(key, root);
	}
	
	private Value get(Key key, Node x){
		if(x == null) return null;
		int cmp = doCompare(key, x);
		if(cmp < 0) return get(key, x.less);
		else if(cmp > 0) return get(key, x.more);
		else return x.val;
	}
	
	public void put(Key key, Value val){
		root = put(key, val, root, 0);
	}
	
	private Node put(Key key, Value val, Node x, int depth) {
		if(x == null) return new Node(key, val, depth);
		int cmp = doCompare(key, x);
		if(cmp == 0)		x.val = val;
		else if(cmp < 0)	x.less = put(key, val, x.less, depth+1);
		else				x.more = put(key, val, x.more, depth+1);
		return x;
	}
	
	private int doCompare(Key key, Node x){
		Comparator<Key> c = comps.get(x.depth % comps.size());
		return c.compare(key, x.key);
	}
	
	public Iterable<Key> keys(){
		Stack<Key> s = new Stack<>();
		keys(root, s);
		return s;
	}

	private void keys(Node x, Stack<Key> s) {
		if(x==null) return;
		s.push(x.key);
		keys(x.less, s);
		keys(x.more, s);
	}

	public Iterable<Key> keys(Key low, Key high) {
		Stack<Key> s = new Stack<>();
		return s;
	}

}
