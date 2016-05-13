﻿using UnityEngine;
using System.Collections;
using org.rnp.voxel.translator;
using org.rnp.voxel.utils;
using org.rnp.voxel.mesh;
using org.rnp.voxel;
using System;

public class Water : VoxelMeshContainer
{
  public VoxelMesh mesh = new MapVoxelMesh(new Dimensions3D(16, 16, 16));
  public VoxelRenderer vr;
  public MeshRenderer mr;

  public int width, height;
  public Vector3 position;

  public float tickTime = 1;
  private float timeLeft;

  public override VoxelMesh Mesh
  {
    get
    {
      return mesh;
    }
  }

  private static int[] table = new int[] {
      0,  8, 16, 24, 32, 40, 48, 56, 64, 72, 80, 88, 96,104,112,120,128,136,144,152,160,
    168,176,184,192,200,208,216,224,232,240,248,256,264,272,280,288,296,304,312,320,328,
    336,344,352,360,368,376,384,392,400,408,416,424,432,440,448,456,464,472,480,488,496,
    504, 64, 72, 80, 88, 96,104,112,120,192, 73,208, 89,224,105,240,121,192,200,208,216,
    224,232,240,248,200,201,216,217,232,233,248,249,320,328,336,344,352,360,368,376,448,
    329,464,345,480,361,496,377,448,456,464,472,480,488,496,504,464,457,488,473,488,489,
    504,505,128,136,144,152,160,168,176,184,192,200,208,216,224,232,240,248,384,392,400,
    408,224,184,240,186,448,456,464,472,240,248,496,504,384,392,400,408,416,424,432,440,
    448,456,464,472,480,488,496,504,448,408,464,440,480,440,496,442,464,472,488,504,496,
    504,504,506,192,200,208,216,224,232,240,248,448,201,216,217,240,233,248,249,448,456,
    464,472,240,248,496,504,456,457,472,473,248,249,504,505,448,456,464,472,480,488,496,
    504,456,457,472,473,488,489,504,505,464,472,472,504,496,504,504,506,472,473,504,475,
    504,505,506,507,256,264,272,280,288,296,304,312,320,328,336,344,352,360,368,376,384,
    392,400,408,416,424,432,440,448,456,464,472,480,488,496,504,384,392,400,408,292,300,
    308,316,448,456,464,472,356,364,372,380,416,424,432,440,420,428,436,444,480,488,496,
    504,484,492,500,508,320,328,336,344,352,360,368,376,448,329,464,345,480,361,496,377,
    448,456,464,472,480,488,496,504,456,457,472,473,488,489,504,505,448,456,464,472,356,
    364,372,380,464,457,376,473,484,365,500,381,480,488,496,504,484,492,500,508,488,489,
    504,505,492,493,508,509,384,392,400,408,416,424,432,440,448,456,464,472,480,488,496,
    504,448,456,464,472,480,488,496,504,464,472,482,504,496,504,504,506,448,424,432,440,
    420,428,436,444,480,488,496,504,484,492,500,508,480,440,496,504,484,444,500,508,496,
    504,504,506,500,508,508,510,448,456,464,472,480,488,496,504,464,457,472,473,488,489,
    504,505,464,472,496,504,496,504,504,506,488,473,504,505,504,505,506,507,480,488,496,
    504,484,492,500,508,488,489,504,505,492,493,508,509,488,504,504,506,500,508,508,510,
    504,505,506,507,508,509,510,511
  };

  // Use this for initialization
  void Start()
  {
    timeLeft = 0;
  }

  // Update is called once per frame
  void Update()
  {
    timeLeft += Time.deltaTime;

    if(timeLeft > tickTime)
    {
      while (timeLeft > tickTime)
      {
        this.Simulate();
        timeLeft -= tickTime;
      }
      mesh.Commit();
    }
  }

  public void Simulate()
  {
    for (int i = 0; i < width; i++)
    {
      for (int j = 1; j < height; j++)
      {
        VoxelLocation partLoc = new Vector3(i, j, 0);
        ComputeLife(partLoc);
      }
    }
  }

  private void ComputeLife(VoxelLocation cell)
  {
    Apply(cell, Next(Capture(cell)));
  }

  private int Capture(VoxelLocation cell)
  {
    int state = 0;
    if (IsAlive(cell.Add(-1, 1, 0)))
      state |= 1;
    if (IsAlive(cell.Add(0, 1, 0)))
      state |= 2;
    if (IsAlive(cell.Add(1, 1, 0)))
      state |= 4;
    if (IsAlive(cell.Add(-1, 0, 0)))
      state |= 8;
    if (IsAlive(cell))
      state |= 16;
    if (IsAlive(cell.Add(1, 0, 0)))
      state |= 32;
    if (IsAlive(cell.Add(-1, -1, 0)))
      state |= 64;
    if (IsAlive(cell.Add(0, -1, 0)))
      state |= 128;
    if (IsAlive(cell.Add(1, -1, 0)))
      state |= 256;
    return state;
  }

  private void Apply(VoxelLocation cell, int state)
  {
    if ((1 & state) != 0)
      this.Live(cell.Add(-1, 1, 0));
    else
      Kill(cell.Add(-1, 1, 0));

    if ((2 & state) != 0)
      this.Live(cell.Add(0, 1, 0));
    else
      Kill(cell.Add(0, 1, 0));

    if ((4 & state) != 0)
      this.Live(cell.Add(1, 1, 0));
    else
      Kill(cell.Add(1, 1, 0));

    if ((8 & state) != 0)
      this.Live(cell.Add(-1, 0, 0));
    else
      Kill(cell.Add(-1, 0, 0));

    if ((16 & state) != 0)
      this.Live(cell);
    else
      Kill(cell);

    if ((32 & state) != 0)
      this.Live(cell.Add(1, 0, 0));
    else
      Kill(cell.Add(1, 0, 0));

    if ((64 & state) != 0)
      this.Live(cell.Add(-1, -1, 0));
    else
      Kill(cell.Add(-1, -1, 0));

    if ((128 & state) != 0)
      this.Live(cell.Add(0, -1, 0));
    else
      Kill(cell.Add(0, -1, 0));

    if ((256 & state) != 0)
      this.Live(cell.Add(1, -1, 0));
    else
      Kill(cell.Add(1, -1, 0));
  }

  private int Next(int state)
  {
    return Water.table[state];
  }

  public void createParticule(int posx, int posy)
  {
    createParticule(new VoxelLocation(posx, posy, 0));
  }

  public void createParticule(VoxelLocation _position)
  {
    Live(_position);
  }

  private void Live(VoxelLocation cell)
  {
    mesh[cell] = new Color32(0, 0, 255, 0);
  }

  private void Kill(VoxelLocation cell)
  {
    mesh[cell] = Voxels.Empty;
  }

  private bool IsAlive(VoxelLocation locToTest)
  {
    return Voxels.HasNegative(locToTest)
          || locToTest.X > (position.x + width)
          || locToTest.Y > (position.y + height)
          || Voxels.IsNotEmpty(mesh[locToTest]);
  }

  private bool IsDead(VoxelLocation locToTest)
  {
    return !IsAlive(locToTest);
  }
}