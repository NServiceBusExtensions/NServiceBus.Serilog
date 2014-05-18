namespace Server
{
    using NServiceBus;

    class MessageSender : IWantToRunWhenBusStartsAndStops
    {
        public IBus Bus { get; set; }

        public void Start()
        {
            Bus.SendLocal(new CreateUser
                {
                    UserName = "jsmith",
                    FamilyName = "Smith",
                    GivenNames = "John",
                });
        }

        public void Stop()
        {
        }
    }
}