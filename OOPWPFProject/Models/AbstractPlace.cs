using System;
using System.Collections.Generic;
using System.Text;

namespace OOPWPFProject.Models;

public abstract class AbstractPlace
{
    public virtual required string Name { get; set; }
    public virtual required string Country { get; set; }
    public virtual required string Description { get; set; }

    public abstract string GetDetails();
}
