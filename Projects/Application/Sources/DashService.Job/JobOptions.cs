namespace DashService.Job
{
    public class JobOptions
    {
        public JobHeader JobHeader
        {
            get;
            set;
        }
    }

    public class JobHeader
    {
        public string Name
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }
    }
}
