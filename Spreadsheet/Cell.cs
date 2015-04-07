using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpreadsheetUtilities;

namespace SS
{
    /// <summary>
    ///  Namgi Yoon u0759547
    ///  This class works 
    /// </summary>
    class Cell
    {
        // surt for cell
        private String name;
        private Object content;
        private Object value;
 
       
        /// <summary>
        /// a single cell consists of cellname and cellcontent.
        /// cellname has to be valid like A1, C3 etc.
        /// cellcontent can be num(double), formula, and String.
        /// 
        /// PS5 updated
        /// 
        /// I added lookup delegate to check the content is formula or not 
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        /// <param name="lookup"></param>
        public Cell(String name, Object content, Func<string, double> _lookup)
        {
            this.name = name;
            this.content = content;
           
            if (content is Formula || content is double)
            {
                Formula f = new Formula(content.ToString());
                this.value = f.Evaluate(_lookup);
            }
            else
            {
                this.value = content;
            }
        }

        /// <summary>
        /// get cellvalue
        /// </summary>
        /// <returns></returns>
        public Object getValue() { return this.value; }

        /// <summary>
        /// get cellcontent
        /// </summary>
        /// <returns></returns>
        public Object getContent() { return this.content; }        
       
    }
}
