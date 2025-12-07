namespace History
{
    [System.Serializable]
    public class HeartsData
    {
        public int currentHearts;

        public static HeartsData Capture()
        {
            HeartsData data = new HeartsData();
            data.currentHearts = HeartsManager.instance != null ? HeartsManager.instance.CurrentHearts : 0;
            return data;
        }

        public static void Apply(HeartsData data)
        {
            if (HeartsManager.instance != null)
            {
                HeartsManager.instance.SetHearts(data.currentHearts);
            }
        }
    }
}
