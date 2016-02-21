using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OrbItProcs
{
    public struct ShaderPack
    {
        // Shader variables
        public float[] colour;
        public int enabled;

        public static ShaderPack Default = new ShaderPack(Color.Red);
        public ShaderPack(Color c)
        {
            colour = new float[4] { c.R, c.G, c.B, c.A };
            enabled = 0;
        }
    }

    /// <summary>
    /// Applies a shader to the nodes
    /// </summary>
    [Info(UserLevel.Developer, "Applies a shader to the nodes", CompType)]
    public class Shader : Component
    {
        public const mtypes CompType = mtypes.draw;
        public override mtypes compType { get { return CompType; } set { } }
        public enum ShaderType
        {
            halfscreen,
            glow,
            noise,
            diffuse
        }
        public ShaderPack shaderPack;

        /// <summary>
        /// Changes the node based on a shader
        /// translucency: Overrides the node's opacity
        /// </summary>
        [Info(UserLevel.User, "Changes the node based on a shader \nDiffuse: Overrides the node's diffuseness.")]
        public ShaderType shaderType { get; set; }

        /// <summary>
        /// If enabled, the color's translucency changes to this value (0-255).
        /// </summary>
        /// 
        [Info(UserLevel.User, "If enabled, the color's diffuse changes to this value (0-255).")]
        public Toggle<int> diffuse { get; set; }

        //public static float startx;
        //public static float endx;

        public Shader() : this(null) { }
        public Shader(Node parent = null)
        {
            shaderPack.enabled = 1;

            if (parent != null) this.parent = parent;
            // On creation, shader is enabled
            diffuse = new Toggle<int>(128, false);
        }
        public override void Draw()
        {
            Color c = this.parent.body.color;

            this.shaderPack.colour = new float[4] { c.R, c.G, c.B, c.A };

            this.shaderPack.enabled = diffuse.enabled ? 1 : 0;
        }

    }
}