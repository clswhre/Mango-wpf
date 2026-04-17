using System;
using System.Collections.Generic;
using System.Text;

namespace OOPWPFProject.Models;

public abstract class AbstractPlace
{
    public virtual string Name { get; set; }
    public virtual string Country { get; set; }
    public virtual string Description { get; set; }

    public abstract string GetDetails();
}
