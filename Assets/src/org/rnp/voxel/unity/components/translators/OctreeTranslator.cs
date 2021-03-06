﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using org.rnp.voxel.translator;
using org.rnp.voxel.mesh;
using org.rnp.voxel.mesh.octree;
using org.rnp.voxel.utils;
using org.rnp.voxel.unity.components.meshes;
using UnityEngine;

namespace org.rnp.voxel.unity.components.translators
{
  /// <author>Cédric DEMONGIVERT [cedric.demongivert@gmail.com]</author>
  /// <summary>
  ///   A simple protoypic translator. Do not use in final games.
  /// </summary>
  [ExecuteInEditMode]
  public class OctreeTranslator : PrototypeTranslator
  {
    /// <see cref="org.rnp.voxel.translator.ITranslator"/>
    public override void Translate()
    {
      this.Clear();

      if (this.VoxelMesh == null || this.VoxelMesh.Mesh == null)
      {
        return;
      }

      if (this.VoxelMesh.Mesh is IOctreeVoxelMesh)
      {
        this.TranslateTree(this.VoxelMesh.Mesh.Start, (IOctreeVoxelMesh) this.VoxelMesh.Mesh);
      }
      else
      {
        this.TranslateLeaf(this.VoxelMesh.Mesh.Start, this.VoxelMesh.Mesh);
      }
    }

    /// <summary>
    ///   Translate an octree node.
    /// </summary>
    /// <param name="root"></param>
    /// <param name="octree"></param>
    public void TranslateTree(VoxelLocation root, IOctreeVoxelMesh octree)
    {
      VoxelLocation nextRoot = new VoxelLocation();
      for (int x = 0; x < 2; ++x)
      {
        for (int y = 0; y < 2; ++y)
        {
          for (int z = 0; z < 2; ++z)
          {
            IVoxelMesh child = octree.GetChild(x, y, z);
            if (child != null)
            {
              nextRoot.Set(root).Add(
                x * octree.Width / 2,
                y * octree.Height / 2,
                z * octree.Depth / 2
              );

              if (child is IOctreeVoxelMesh)
              {
                this.TranslateTree(nextRoot, (IOctreeVoxelMesh)child);
              }
              else
              {
                this.TranslateLeaf(nextRoot, child);
              }
            }
          }
        }
      }
    }

    /// <summary>
    ///   Translate an octree leaf.
    /// </summary>
    /// <param name="root"></param>
    /// <param name="mesh"></param>
    public void TranslateLeaf(VoxelLocation root, IVoxelMesh mesh)
    {
      VoxelLocation start = mesh.Start;
      VoxelLocation end = mesh.End;
      VoxelLocation finalLocation = new VoxelLocation();
      
      for (int x = start.X; x < end.X; ++x)
      {
        for (int y = start.Y; y < end.Y; ++y)
        {
          for (int z = start.Z; z < end.Z; ++z)
          {
            finalLocation.Set(root).Add(x,y,z);
            this.Translate(finalLocation, this.VoxelMesh.Mesh, finalLocation);
          }
        }
      }
    }
  }
}
