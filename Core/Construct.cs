namespace Core
{
    public static class Construct
    {
        #region Helper Construct
        public static void Help(object instance)
        {
            var type = instance.GetType();
            RoslynContext.PrintType(type);
        }
        #endregion
    }
}
