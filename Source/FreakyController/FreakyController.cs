using System;
using FlaxEngine;
using KCC;

namespace FreakyController
{
    /// <summary>
    /// The sample game plugin.
    /// </summary>
    /// <seealso cref="FlaxEngine.GamePlugin" />
    [PluginLoadOrder(InitializeAfter = typeof(KCC.KCC))]
    public class FreakyController : GamePlugin
    {
        /// <inheritdoc />
        public FreakyController()
        {
            _description = new PluginDescription
            {
                Name = "FreakyController",
                Category = "Other",
                Author = "ArtemPindrus",
                AuthorUrl = null,
                HomepageUrl = null,
                RepositoryUrl = "https://github.com/FlaxEngine/FreakyController",
                Description = "This is an example plugin project.",
                Version = new Version(),
                IsAlpha = false,
                IsBeta = false,
            };
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <inheritdoc />
        public override void Deinitialize()
        {
            // Use it to cleanup data

            base.Deinitialize();
        }
    }
}
