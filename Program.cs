using System.Collections.Generic;
using System.Linq;
using Masuit.Tools.Models;
using testdemo.manaulimplement;
using testdemo.models;
using testdemo.useihierarchical;

namespace testdemo;

public class Test
{
    public static void Main(string[] args)
    {
        List<Info> infos = Mock();

        //自己实现树与列表结构互转
        List<NodeInfo> treeNodes = TreeUtil.BuildTree(infos);

        var flattenList = TreeUtil.Flatten(treeNodes, true);

        
        var originNodes = infos.Select(x => new NodeInfo()
        {
            id = x.id, pid = x.pid.HasValue?x.pid.Value:-1, label = x.label
        }).ToList();
        
        
        #region 使用Ihierarchical

        //使用IHierarchical工具类

        List<IHierarchical<NodeInfo>> treeNodes1 = new List<IHierarchical<NodeInfo>>();
        foreach (var rootNode in originNodes.Where(x=>x.pid==-1))
        {
            var treeNode=rootNode.AsHierarchical(SelectorChildren);
            treeNodes1.Add(treeNode);
        }

        var flattenList1 = new List<NodeInfo>();
        foreach (var nodeInfo in treeNodes1)
        {
            flattenList1.AddRange(nodeInfo.AsEnumerable(EnumerateType.Bfs).Select(x=>x.Current));
        }
        
        IEnumerable<NodeInfo> SelectorChildren(NodeInfo nodeInfo)
        {
            return originNodes.Where(x => x.pid == nodeInfo.id);
        }

        #endregion

        
        
        #region 使用ITree

        var treeNode2=  originNodes.ToTreeGeneral(_=>_.id, _=>_.pid,-1);
        var flattenListBfs2 = treeNode2.Flatten()?.Select(x=>x.Value).ToList();

        #endregion
    }




    static List<Info> Mock()
    {
        List<Info> infos = new List<Info>();
        infos.Add(new Info() { id = 1, label = "一级 1" });
        infos.Add(new Info() { id = 2, label = "一级 2" });
        infos.Add(new Info() { id = 3, label = "一级 3" });
        infos.Add(new Info() { id = 4, label = "二级 1-1", pid = 1 });
        infos.Add(new Info() { id = 5, label = "二级 2-1", pid = 2 });
        infos.Add(new Info() { id = 6, label = "二级 2-2", pid = 2 });
        infos.Add(new Info() { id = 7, label = "二级 3-1", pid = 3 });
        infos.Add(new Info() { id = 8, label = "二级 3-2", pid = 3 });
        infos.Add(new Info() { id = 9, label = "三级 1-1-1", pid = 4 });
        infos.Add(new Info() { id = 10, label = "三级 1-1-2", pid = 4 });
        return infos;
    }
}