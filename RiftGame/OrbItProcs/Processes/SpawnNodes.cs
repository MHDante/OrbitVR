using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
namespace OrbItProcs
{
    public class SpawnNodes : Process
    {
        private Vector2 spawnPos;
        int rightClickCount = 0;//
        int rightClickMax = 1;//
        public int batchSpawnNum { get; set; }

        public Toggle<float> radiusRange { get; set; }
        public float radiusCenter { get; set; }

        public SpawnNodes() : base()
        {
//             List<int> a = new List<int>();
//             List<int> b = new List<int>();
//             a.

            batchSpawnNum = 2;
            radiusRange = new Toggle<float>(10f, true);
            radiusCenter = 15f;

            addProcessKeyAction("SpawnNode", KeyCodes.LeftClick, OnPress: SpawnNode);
            //addProcessKeyAction("SetSpawnPosition", KeyCodes.LeftShift, OnPress: SetSpawnPosition);
            addProcessKeyAction("BatchSpawn", KeyCodes.RightClick, OnHold: BatchSpawn);
            //addProcessKeyAction("DirectionalLaunch", KeyCodes.LeftShift, KeyCodes.RightClick, OnHold: DirectionalLaunch);
            
            addProcessKeyAction("testing", KeyCodes.OemPipe, OnPress: TestingStuff);
        }

        public void SetRadius(Node n)
        {
            n.body.radius = radiusCenter;
            if (radiusRange.enabled)
            {
                n.body.radius = (float)Utils.random.NextDouble() * radiusRange.value - (radiusRange.value / 2) + radiusCenter;
            }
        }

        public void SpawnNode()
        {
            SetRadius(room.spawnNode((int)UserInterface.WorldMousePos.X, (int)UserInterface.WorldMousePos.Y));
        }
        #region Testing Region
        public void TestingStuff()
        {
            //
            //room.game.testing.TestOnClick();
            //room.game.testing.TestHashSet();
            //room.game.testing.WhereTest();
            //room.game.testing.ForLoops();
            //room.game.testing.ColorsTest();
            //room.game.testing.NormalizeTest();
            //DateTime before = DateTime.Now;
            //room.game.testing.LoopTesting();
            //
            //int diff = DateTime.Now.Millisecond - before.Millisecond;
            //if (diff < 0)  diff += 1000;
            //Console.WriteLine("DIFF:" + diff);
            //

            //testing.TriangleTest2();
            //testing.TestRedirect();
            //room.game.testing.Gridsystem();
            //room.gridsystem.GenerateReachOffsets();
            //room.gridsystem.PrintOffsets(10);
            //long totalMemory = GC.GetTotalMemory(true);
            //Console.WriteLine(totalMemory);
            //room.gridsystem.GenerateAllReachOffsetsPerCoord(300);
            //long totalMemoryNew = GC.GetTotalMemory(true);
            //Console.WriteLine(totalMemoryNew);
            //
            //float m1 = Testing.ByteToMegabyte((int)totalMemory);
            //float m2 = Testing.ByteToMegabyte((int)totalMemoryNew);
            //Console.WriteLine("{0} - {1} = {2}", m2, m1, m2 - m1);
            //room.gridsystem.PrintOffsets(max: 300, x: 0, y: 0);
            //Testing.IndexsGridsystem();
            //Testing.TestCountArray();
            Testing.shittest();
        }
        #endregion

        public void SetSpawnPosition()
        {
            spawnPos = UserInterface.WorldMousePos;
        }
        public void BatchSpawn()
        {
            int rad = 100;
            for (int i = 0; i < batchSpawnNum; i++)
            {
                int rx = Utils.random.Next(rad * 2) - rad;
                int ry = Utils.random.Next(rad * 2) - rad;
                SetRadius(room.spawnNode((int)UserInterface.WorldMousePos.X + rx, (int)UserInterface.WorldMousePos.Y + ry));
            }
        }

        public void DirectionalLaunch()
        {
            rightClickCount++;
            if (rightClickCount % rightClickMax == 0)
            {
                //Vector2 positionToSpawn = new Vector2(Game1.sWidth, Game1.sHeight);
                Vector2 positionToSpawn = spawnPos;
                //positionToSpawn /= (game.room.mapzoom * 2);
                //positionToSpawn /= (2);
                Vector2 diff = UserInterface.WorldMousePos;
                //diff *= room.zoom;
                diff = diff - positionToSpawn;
                //diff.Normalize();

                //new node(s)
                Dictionary<dynamic, dynamic> userP = new Dictionary<dynamic, dynamic>() {
                                
                            { typeof(Lifetime), true },
                            { nodeE.position, positionToSpawn },
                            { nodeE.velocity, diff },
                        };

                if (UserInterface.oldKeyBState.IsKeyDown(Keys.LeftControl))
                {
                    Action<Node> after = delegate(Node n) { 
                        n.body.velocity = diff;
                        if (n.body.velocity.IsFucked()) System.Diagnostics.Debugger.Break();
                    
                    }; 
                    SetRadius(room.spawnNode(userP, after));
                }
                else
                {
                    SetRadius(room.spawnNode(userP));
                }
                rightClickCount = 0;
            }
        }
    }
}
