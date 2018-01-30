[System.Serializable]
public class ArrayLayout  {

	[System.Serializable]
	public struct rowData{
		public Number[] column;
        public PowerHolder[] pColumn;
	}

	public rowData[] row = new rowData[5]; //Grid of 7x7
}
