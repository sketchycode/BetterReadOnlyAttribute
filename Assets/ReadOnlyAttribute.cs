using UnityEngine;

public class ReadOnlyAttribute : PropertyAttribute {
    public readonly string grouping;

    public ReadOnlyAttribute (string grouping) {
		this.grouping = grouping;
    }
}
