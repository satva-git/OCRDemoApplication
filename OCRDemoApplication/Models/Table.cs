using Amazon.Textract.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OCRDemoApplication.Models
{
    public class Table
    {
        public Table(Block block, Dictionary<string, Block> blocks)
        {
            this.Block = block;
            this.Confidence = block.Confidence;
            this.Geometry = block.Geometry;
            this.Id = block.Id;
            this.Rows = new List<Row>();
            var ri = 1;
            var row = new Row();

            var relationships = block.Relationships;
            if (relationships != null && relationships.Count > 0)
            {
                relationships.ForEach(r =>
                {
                    if (r.Type == "CHILD")
                    {
                        r.Ids.ForEach(id =>
                        {
                            var cell = new Cell(blocks[id], blocks);
                            if (cell.RowIndex > ri)
                            {
                                this.Rows.Add(row);
                                row = new Row();
                                ri = cell.RowIndex;
                            }
                            row.Cells.Add(cell);
                        });
                        if (row != null && row.Cells.Count > 0)
                            this.Rows.Add(row);
                    }
                });
            }
        }
        public List<Row> Rows { get; set; }
        public Block Block { get; set; }
        public float Confidence { get; set; }
        public Geometry Geometry { get; set; }
        public string Id { get; set; }

        public override string ToString()
        {
            var result = new List<string>();
            result.Add(string.Format("Table{0}===={0}", Environment.NewLine));
            this.Rows.ForEach(r =>
            {
                result.Add(string.Format("Row{0}===={0}{1}{0}", Environment.NewLine, r));
            });
            return string.Join("", result);
        }
    }

    public class Word
    {
        public Word(Block block, Dictionary<string, Block> blocks)
        {
            this.Block = block ?? new Block();
            this.Blocks = blocks ?? new Dictionary<string, Block>();
            this.Confidence = block == null ? 0 : block.Confidence;
            this.Geometry = block == null ? new Geometry() : block.Geometry;
            this.Id = block == null ? string.Empty : block.Id;
            this.Text = block == null ? string.Empty : block.Text;
        }

        public Block Block { get; set; }
        public Dictionary<string, Block> Blocks { get; set; }
        public float Confidence { get; set; }
        public Geometry Geometry { get; set; }
        public string Id { get; set; }
        public string Text { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }

    public class Row
    {
        public Row()
        {
            this.Cells = new List<Cell>();
        }
        public List<Cell> Cells { get; set; }

        public override string ToString()
        {
            var result = new List<string>();
            this.Cells.ForEach(c =>
            {
                result.Add(string.Format("[{0}]", c));
            });
            return string.Join("", result);
        }
    }

    public class Cell
    {
        public Cell(Block block, Dictionary<string, Block> blocks)
        {
            if (block == null)
                return;
            this.Block = block;
            this.ColumnIndex = block.ColumnIndex;
            this.ColumnSpan = block.ColumnSpan;
            this.Confidence = block.Confidence;
            this.Content = new List<dynamic>();
            this.Geometry = block.Geometry;
            this.Id = block.Id;
            this.RowIndex = block.RowIndex;
            this.RowSpan = block.RowSpan;
            this.Text = string.Empty;

            var relationships = block.Relationships;
            if (relationships != null && relationships.Count > 0)
            {
                relationships.ForEach(r =>
                {
                    if (r.Type == "CHILD")
                    {
                        r.Ids.ForEach(id =>
                        {
                            var rb = blocks[id];
                            if (rb != null && rb.BlockType == "WORD")
                            {
                                var w = new Word(rb, blocks);
                                this.Content.Add(w);
                                this.Text = this.Text + w.Text + " ";
                            }
                            else if (rb != null && rb.BlockType == "SELECTION_ELEMENT")
                            {
                                var se = new SelectionElement(rb, blocks);
                                this.Content.Add(se);
                                this.Text = this.Text + se.SelectionStatus + ", ";
                            }
                        });
                    }

                });
            }
        }
        public int RowIndex { get; set; }
        public int RowSpan { get; set; }
        public int ColumnIndex { get; set; }
        public int ColumnSpan { get; set; }
        public List<dynamic> Content { get; set; }
        public Block Block { get; set; }
        public float Confidence { get; set; }
        public Geometry Geometry { get; set; }
        public string Id { get; set; }
        public string Text { get; set; }

        public override string ToString()
        {
            return this.Text;
        }
    }

    public class SelectionElement
    {
        public SelectionElement(Block block, Dictionary<string, Block> blocks)
        {
            this.Confidence = block.Confidence;
            this.Geometry = block.Geometry;
            this.Id = block.Id;
            this.SelectionStatus = block.SelectionStatus;
        }
        public float Confidence { get; set; }
        public Geometry Geometry { get; set; }
        public string Id { get; set; }
        public string SelectionStatus { get; set; }

    }
}