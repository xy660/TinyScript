using ScriptRuntime.Core;
using ScriptRuntime.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class SyntaxUtils
{
    public static string SymbolMap = "+-*/||&&%!.^\\=><:";
    public static List<string> MathSymbol = new List<string>()
    {
        "+",
        "-",
        "*",
        "/",
    };
    public static List<string> LogicSymbol = new List<string>()
    {
        "==",
        "||",
        "&&",
        "!"
    };
    public static string ClipTokenString(int start, int count,List<Token> ASTParseStream)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = start; i < start + count - 1; i++)
        {
            if (i < ASTParseStream.Count && i >= 0)
            {
                if (ASTParseStream[i].tokenType == TokenType.Part)
                {
                    sb.Append($"({ASTParseStream[i].raw})");
                }
                else if (ASTParseStream[i].tokenType == TokenType.IndexLabel)
                {
                    sb.Append($"[{ASTParseStream[i].raw}]");
                }
                else
                {
                    sb.Append(ASTParseStream[i].raw);
                }
                sb.Append(' ');
            }
        }
        return sb.ToString();
    }
    public static string GetASTString(ASTNode node, string indent = "", bool isLast = true,StringBuilder buf = null)
    {
        StringBuilder sb = buf is null ? new StringBuilder() : buf;
        // 打印当前节点
        sb.Append(indent);
        if (isLast)
        {
            sb.Append("└─ ");
            indent += "   ";
        }
        else
        {
            sb.Append("├─ ");
            indent += "│  ";
        }
        sb.AppendLine($"{AOTEnumMap.ASTNodeTypeString[node.NodeType]}: {node.Raw}");

        // 递归打印子节点
        for (int i = 0; i < node.Childrens.Count; i++)
        {
            GetASTString(node.Childrens[i], indent, i == node.Childrens.Count - 1,sb);
        }
        return sb.ToString();
    }
}
