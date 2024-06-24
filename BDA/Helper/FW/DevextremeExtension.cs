using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BDA.Helper
{
    public static class DXExt
    {
        public static DataGridBuilder<T> WSDataGrid<T>(this DataGridBuilder<T> grid)
        {
            grid.Width("100%");
            grid.FilterRow(f => f.Visible(true));
            grid.ShowBorders(false);
            grid.Selection(c => c.Mode(SelectionMode.Single));
            grid.RowAlternationEnabled(false);
            grid.Paging(p =>
            {
                p.PageSize(20);
                p.Enabled(true);
            }
            );
            return grid;
        }

        public static DataGridBuilder<T> EditableDataGrid<T>(this DataGridBuilder<T> grid, bool hasUpdateAccess = true)
        {
            grid.Width("100%");
            grid.ColumnAutoWidth(true);
            grid.ShowRowLines(true);
            grid.AllowColumnReordering(true);
            //grid.FilterRow(f => f.Visible(false));
            //grid.FilterPanel(f => f.Visible(false));
            grid.ShowBorders(false);

            grid.RowAlternationEnabled(false);
            grid.WordWrapEnabled(false);
            grid.RemoteOperations(true);
            grid.AllowColumnResizing(true);
            grid.ColumnResizingMode(ColumnResizingMode.Widget);
            grid.Paging(p =>
            {
                p.PageSize(10);
                p.Enabled(true);
            });
            grid.Pager(p =>
            {
                p.ShowInfo(true);
            }
            );
            grid.Selection(c => c.Mode(SelectionMode.Single));            
            grid.Editing(editing =>
            {
                editing.UseIcons(true);
                editing.Mode(GridEditMode.Popup);
                editing.AllowAdding(false);
                editing.AllowDeleting(false);
                editing.AllowUpdating(hasUpdateAccess);
            });
            return grid;
        }

    }
}
