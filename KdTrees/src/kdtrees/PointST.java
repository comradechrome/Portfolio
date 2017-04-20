package kdtrees;

import edu.princeton.cs.algs4.Point2D;
import edu.princeton.cs.algs4.Queue;
import edu.princeton.cs.algs4.RedBlackBST;
import edu.princeton.cs.introcs.In;
import edu.princeton.cs.introcs.StdOut;

public class PointST<Value> {

	private RedBlackBST<Point2D, Value> rbst;

	public PointST(){ // construct an empty symbol table of points
		rbst = new RedBlackBST<>();
	}

	public boolean isEmpty(){ // is the symbol table empty?
		return (rbst.size() == 0);
	}

	public int size(){ // number of points
		return rbst.size();
	}

	public void put(Point2D p, Value val){ // associate the value val with point p
		rbst.put(p, val);
	}

	public Value get(Point2D p){ // value associated with point p
		return rbst.get(p);
	}

	public boolean contains(Point2D p){ // does the symbol table contain point p?
		return (rbst.contains(p));
	}

	public Iterable<Point2D> points(){// all points in the symbol table
		return rbst.keys();
	}

	public Iterable<Point2D> range(RectHV rect){ // all points that are inside the rectangle
		Queue<Point2D> queue = new Queue<>();
		Point2D low = new Point2D(rect.xmin(), rect.ymin());
		Point2D high = new Point2D(rect.xmax(), rect.ymax());
		for(Point2D p : rbst.keys(low, high)){		//keys() only provides y resolution
			if(rect.contains(p)){	//therefore we still filter on x
				queue.enqueue(p);
			}
		}
		return queue;
	}

	public Point2D nearest(Point2D p){ // a nearest neighbor to point p; null if the symbol table is empty
		Point2D winner = null;
		double best = Double.MAX_VALUE;
		for(Point2D x : rbst.keys()){
			double distance = x.distanceSquaredTo(p);
			winner = (distance<best)?x:winner;
			best = (distance<best)?distance:best;
		}
		return winner;
	}

	public static void main(String[] args){                  // unit testing of the methods (not graded)
		String filename = args[0];
		In in = new In(filename);
		PointST<Integer> pst = new PointST<>();
		for (int i = 0; !in.isEmpty(); i++) {
			double x = in.readDouble();
			double y = in.readDouble();
			Point2D d = new Point2D(x, y);
			pst.put(d, i);
		}

		StdOut.println(pst.nearest(new Point2D(.2, .5)));
		StdOut.println();
		Iterable<Point2D> list = pst.range(new RectHV(.25, .25, .30, .30));
		for(Point2D p : list){
			StdOut.println(p);
		}
	}
}
