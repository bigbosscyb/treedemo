using System.Collections.Generic;
using System.Linq;
using testdemo.models;

namespace testdemo.manaulimplement;

public static class TreeUtil
{
    /// <summary>
    /// 树转平整结构
    /// </summary>
    /// <param name="trees"></param>
    /// <returns></returns>
    public static List<NodeInfo> Flatten(List<NodeInfo> trees, bool usebfs)
    {
        return usebfs ? FlattenWithBFS(trees) : FlattenWithDFS(trees);
    }

    private static List<NodeInfo> FlattenWithBFS(List<NodeInfo> trees)
    {
        if (trees == null || trees.Count == 0)
        {
            return null;
        }

        var list = new List<NodeInfo>();

        Queue<NodeInfo> queue = new Queue<NodeInfo>();

        foreach (var currentTree in trees)
        {
            queue.Enqueue(currentTree);
        }

        while (queue.Any())
        {
            var current = queue.Dequeue();
            list.Add(current);
            if (current.children != null && current.children.Count > 0)
            {
                foreach (var childInfo in current.children)
                {
                    queue.Enqueue(childInfo);
                }
            }
        }

        return list;
    }

    /// <summary>
    /// 平整结构转树结构
    /// </summary>
    /// <param name="trees"></param>
    /// <returns></returns>
    private static List<NodeInfo> FlattenWithDFS(List<NodeInfo> trees)
    {
        if (trees == null || trees.Count == 0)
        {
            return null;
        }

        var list = new List<NodeInfo>();

        Stack<NodeInfo> stack = new Stack<NodeInfo>();

        foreach (var currentTree in trees)
        {
            stack.Push(currentTree);
            while (stack.Count > 0)
            {
                var nodeInfo = stack.Pop();
                list.Add(nodeInfo);
                if (nodeInfo.children != null && nodeInfo.children.Count > 0)
                {
                    nodeInfo.children.Reverse(); //这里要逆序一下 否则下一轮出栈的不是第一个字节点
                    foreach (var childInfo in nodeInfo.children)
                    {
                        stack.Push(childInfo);
                    }
                }
            }
        }

        return list;
    }

    /// <summary>
    /// 平整结构转树结构
    /// </summary>
    /// <param name="infos"></param>
    /// <returns></returns>
    public static List<NodeInfo> BuildTree(List<Info> infos)
    {
        List<NodeInfo> trees = new List<NodeInfo>();
        if (infos == null || infos.Count == 0)
        {
            return trees;
        }

        //使用字典维护节点id与直接子节点集合
        Dictionary<int, List<Info>> childDic = new Dictionary<int, List<Info>>();
        var idList = infos.Select(x => x.id).ToList(); //取出所有id
        foreach (var id in idList)
        {
            if (!childDic.ContainsKey(id))
            {
                childDic[id] = new List<Info>();
            }
        }

        //筛选出拥有子节点的元素
        var hasParentList = infos.Where(x => x.pid != null && childDic.ContainsKey(x.pid.Value)).ToList();
        foreach (var childInfo in hasParentList)
        {
            if (childInfo.pid != null) //将此元素加入父id所在的字典项对应的字元素列表中
            {
                childDic[childInfo.pid.Value].Add(childInfo);
            }
        }

        //筛选出无父级的元素(一棵棵树的根节点)
        var roots = infos.Where(x => x.pid == null).ToList();
        foreach (var root in roots)
        {
            Queue<NodeInfo> queue = new Queue<NodeInfo>();
            var item = new NodeInfo() { id = root.id, label = root.label };
            queue.Enqueue(item);
            while (queue.Any()) //使用队列完成深度遍历
            {
                var current = queue.Dequeue();
                var children = childDic[current.id];
                if (children != null && children.Any())
                {
                    foreach (var childInfo in children)
                    {
                        var childNodeInfo = new NodeInfo() { id = childInfo.id, label = childInfo.label };
                        if (current.children == null)
                        {
                            current.children = new List<NodeInfo>();
                        }

                        current.children.Add(childNodeInfo); //加入子节点列表

                        queue.Enqueue(childNodeInfo); //推动更深一层探测
                    }
                }
            }

            trees.Add(item);
        }

        return trees;
    }
}