using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Domain.entities.common
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
       
    }
}
