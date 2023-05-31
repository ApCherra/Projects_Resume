using System.Diagnostics.SymbolStore;
using System.Numerics;
using Microsoft.VisualBasic.CompilerServices;

namespace SpreadsheetEngine;

public class ExpressionTree
{
    private Dictionary<string, string> var_dict = new Dictionary<string, string>();
    private SpNode root;
    private double sum = 0.0;
    private string expression;
    public ExpressionTree(string infixexpression)
    {
        expression = infixToPostfix(infixexpression); 
        Console.WriteLine("Post Fix Expression " + expression);
        Stack<SpNode> st = new Stack<SpNode>();
        SpNode t1, t2, temp;
        String[] tags = expression.Split();
 //       Console.WriteLine("Number of tags " + tags.Length);
        foreach(var tag in tags)
        {
            if (tag.Equals(" ") || tag.Equals(""))
            {
 //               Console.WriteLine("Next ");
                continue;
            }
                
   //         Console.WriteLine("processing " + tag);
            if(!isOperator(tag))
            {
                if (isVar(tag))
                {
                    var_dict.Add(tag, "0");
                    temp = new NumNode("" + tag);
                }
                else
                {
                    temp = new NumNode(tag);
                }

                st.Push(temp);
            }
            else
            {
                
                temp = new BinaryOperatorNode(tag);
 
                t1 = st.Pop();
                t2 = st.Pop();
 
                temp.left = t2;
                temp.right = t1;
 
                st.Push(temp);
            }
 
        }
        root = st.Pop();
    }

    private bool isOperator(string ch)
    {
        if(ch=="+" || ch=="-"|| ch=="*" || ch=="/" || ch=="^")
        {
            return true;
        }
        return false;
        
    }
    private bool isVar(string ch)
    {
        double d = 0.0;
        try
        {
            Double.Parse(ch);
            return false;
        }
        catch (Exception e)
        {
            return true;
        }

        return true;
    }
    public  string infixToPostfix(string exp)
    {
        // Initializing empty String for result
        string result = "";
  
        // Initializing empty stack
        Stack<string> stack = new Stack<string>();
        string[] tags = exp.Split();
        foreach (string tag in tags) {
           
            // If the scanned character is an
            // operand, add it to output.
            if (tag.Equals("(") || tag.StartsWith("(")) {
                stack.Push("(");
            }
            // If the scanned character is an ')',
            // pop and output from the stack
            // until an '(' is encountered.
            else if (tag.Equals(")") || tag.EndsWith(")")) {
                while (stack.Count > 0
                       && stack.Peek() != "(") {
                    result += " " + stack.Pop();
                }
  
                if (stack.Count > 0
                    && stack.Peek() != "(") {
                    return "Invalid Expression";
                }
                else {
                    stack.Pop();
                }
            }
            
            // An operator is encountered
            else if (isOperator(tag))
            {
                while (stack.Count > 0
                       && Prec(tag) <= Prec(stack.Peek()))
                {
                    result += " " + stack.Pop();
                }

                stack.Push(tag);
            }
            else
            {
                result += " " + tag;
            }
        }
  
        // Pop all the operators from the stack
        while (stack.Count > 0) {
            result += " " + stack.Pop();
        }
  
        return result;
    }
    internal static int Prec(string ch)
    {
        switch (ch) {
            case "+":
            case "-":
                return 1;
  
            case "*":
            case "/":
                return 2;
  
            case "^":
                return 3;
        }
        return -1;
    }

    public void SetVariable(string variableName, string variableValue)
    {
        if(var_dict.ContainsKey(variableName))
            var_dict[variableName] = variableValue;
        else
        {
            var_dict.Add(variableName, variableValue);
        }
        
    }
    

    public int Evaluate()
    {
        return evalTree(root);
    }

    public  int evalTree(SpNode root) {

        if (root == null)
        {
            return 0;
        }
        // Leaf node i.e, an integer
        if (root.left == null && root.right == null)
        {
            Console.WriteLine("Processing Node " + root.node_value);
            var v = root.node_value;
            if (isVar(v))
            {
                v = var_dict.GetValueOrDefault(v, "0");
            }
            int out_val = 0;
            try
            {
                out_val = Int32.Parse(v); 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
            }

            return out_val;
        }

        // Evaluate left subtree
        int leftEval = evalTree(root.left);
 
        // Evaluate right subtree
        int rightEval = evalTree(root.right);
 
        // Check which operator to apply
        //TODO Move this logic to Each Operator Class
        if (root.node_value.Equals("+"))
            return leftEval + rightEval;
 
        if (root.node_value.Equals("-"))
            return leftEval - rightEval;
 
        if (root.node_value.Equals("*"))
            return leftEval * rightEval;
 
        return leftEval / rightEval;
    }

    public List<string> getExpressionVars()
    {
        return var_dict.Keys.ToList();
    }
}

public class SpNode
{
    protected string type;
    protected string var_name;
    public string node_value;
    public SpNode left;
    public SpNode right;
}

public class NumNode: SpNode {
    public NumNode(string nm)
    {
        node_value = nm;
        type = "value";
    }
}

public class BinaryOperatorNode: SpNode 
{
    public BinaryOperatorNode(string op)
    {
        type = "Expression";
        var_name = null;
        node_value = op; 
    }

    public void SetLeft(SpNode lNode)
    {
        left = lNode;
    }

    public void SetRight(SpNode rNode)
    {
        right = rNode;
    }
}
public class VarNode: SpNode 
{
    public VarNode(string var)
    {
        type = "Expression";
        var_name = null;
        node_value = var;
    }

    public void SetLeft(SpNode lNode)
    {
        left = lNode;
    }

    public void SetRight(SpNode rNode)
    {
        right = rNode;
    }
}