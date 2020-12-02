using System.Collections.Generic;

public class Root
{
    public string @ref { get; set; }
    public string before { get; set; }
    public string after { get; set; }
    public bool created { get; set; }
    public bool deleted { get; set; }
    public bool forced { get; set; }
    public object base_ref { get; set; }
    public string compare { get; set; }
    public List<object> commits { get; set; }
    public object head_commit { get; set; }
    public Repository repository { get; set; }
    public Pusher pusher { get; set; }
    public Sender sender { get; set; }
}




