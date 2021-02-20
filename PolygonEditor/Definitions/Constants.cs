using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolygonEditor.Definitions
{
    public static class CONSTS
    {
        public const int areaMaxWidth = 10000;
        public const int areaMaxHeight = 10000;

    }

    public static class TEXTS
    {
        public const string ONE_CONSTRAINT_NOT = "You can add only one constraint to an edge. Delete existing first.";
        public const string INCIDENT_EDGES_VERTICAL_NOT = "A vartical edge cannot be incident to annother vertical edge.";
        public const string INCIDENT_EDGES_HORIZONTAL_NOT = "A horizontal edge cannot be incident to annother horizontal edge.";
        public const string VERTICAL_COORDINATES_NOT = "X cooridates of a vertical edge's vertices must be egual.";
        public const string HORIZONTAL_COORDINATES_NOT = "Y cooridates of a horizontal edge's vertices must be egual.";
        public const string CONSTRAIN_DELETED_NOT = "Constrain has beed deleted.";
        public const string EMPTY_CONSTRAINT_CANNOT_DELETE = "Constraint cannot be deleted, because this edhe has no constraint.";
        public const string EMPTY_CONSTRAINT_CANNOT_APPLY_WITH_CHECK = "Cannot apply with constraint check, because this edge has no constraint.";
        public const string LENGTH_CANNOT_CHANGE = "The length of this edge cannot be changed";
        public const string CANNOT_ADD_OTHER_EDGES = "The constraint cannot be applied because of other edges' constraints.";
        public const string UNKNOWN_ERROR = "Unknown error.";
    }
}
