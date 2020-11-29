using System;

[Serializable]
public class Root 
{
    public string zen { get; set; }
    public int hook_id { get; set; }
    public Hook hook { get; set; }
    public Repository repository { get; set; }
    public Sender sender { get; set; }
}


