namespace Client
{
    struct DestroyEvent
    {
        public int ExposedEntity;

        /// <summary>
        /// Which entity must be destroyed?
        /// </summary>
        /// <param name="exposedEntity">Destroyed entity</param>
        public void Invoke(int exposedEntity)
        {
            ExposedEntity = exposedEntity;
        }
    }
}