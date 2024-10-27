using UnityEngine;

namespace Nach.Tools.Pools
{
    public interface IPoolable
    {
        public GameObject gameObject { get; }

        public void OnCreated();

        public void OnGet()
        {
            gameObject.SetActive(true);
        }

        public void OnRelease()
        {
            gameObject.SetActive(false);
        }
    }
}
