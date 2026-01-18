using System.Collections.Generic;

namespace testdemo.models;

public class Info
{
    public int id { get; set; }
    public int? pid { get; set; }

    public string label { get; set; }
}

public class NodeInfo
{
    public int id { get; set; }
    
    public int pid { get; set; }//使用两个工具库 需要定义此字段

    public string label { get; set; }

    public List<NodeInfo> children { get; set; }
    
}
