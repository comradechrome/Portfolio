package kdtrees;

import java.util.ArrayList;
import java.util.Comparator;

import edu.princeton.cs.algs4.Point2D;
import edu.princeton.cs.introcs.In;
import edu.princeton.cs.introcs.StdOut;

public class KdTreeST<Value>{

	private KdTree<Point2D, Value> kdTree;

	public KdTreeST(){ // construct an empty symbol table of points
		ArrayList<Comparator<Point2D>> c = new ArrayList<>();
		c.add(Point2D.X_ORDER);
		c.add(Point2D.Y_ORDER);
		kdTree = new KdTree<Point2D, Value>(c);
	}

	public boolean isEmpty(){ // is the symbol table empty?
		return (kdTree.size() == 0);
	}

	public int size(){ // number of points
		return kdTree.size();
	}

	public void put(Point2D key, Value val){ // associate the value val with point p
		kdTree.put(key, val);
	}

	public Value get(Point2D key){ // value associated with point p
		return kdTree.get(key);
	}

	public boolean contains(Point2D key){ // does the symbol table contain point p?
		return (kdTree.contains(key));
	}

	public Iterable<Point2D> points(){// all points in the symbol table
		return kdTree.keys();
	}

	public Iterable<Point2D> range(RectHV rect){ // all points that are inside the rectangle
		Point2D low = new Point2D(rect.xmin(), rect.ymin());
		Point2D high = new Point2D(rect.xmax(), rect.ymax());
		return kdTree.keys(low, high);
	}

	public Point2D nearest(Point2D key){ // a nearest neighbor to point p; null if the symbol table is empty
		Point2D winner = null;
		double best = Double.MAX_VALUE;
		for(Point2D x : kdTree.keys()){
			double distance = x.distanceSquaredTo((Point2D) key);
			winner = (distance<best)?x:winner;
			best = (distance<best)?distance:best;
		}
		return winner;
	}

	public static void main(String[] args){                  // unit testing of the methods (not graded)
		In in = new In("/kdtrees/input50.txt");
		KdTreeST<Integer> kdTree = new KdTreeST<>();
		int i = 0;
		while(in.hasNextLine()){
			Double x = in.readDouble();
			Double y = in.readDouble();
			kdTree.put(new Point2D(x, y), i++);
		}
		
		StdOut.println(kdTree.nearest(new Point2D(0, .5)));
		StdOut.println();
		Iterable<Point2D> list = kdTree.range(new RectHV(-.3, .1, .13, .20));
		for(Point2D p : list){
			StdOut.println(p);
		}
	}
}
