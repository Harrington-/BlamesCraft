using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class Chunk : MonoBehaviour {

	public int x;
	public int y;
	public GenerateTerrain Parent;
	public Guid ChunkId;
	public static Dictionary<string, List<GameObject>> availableBlocks = new Dictionary<string, List<GameObject>>();

	public List<GameObject> blocks;

	public Chunk(GenerateTerrain parent, int X, int Y)
	{
		blocks = new List<GameObject>();
		ChunkId = Guid.NewGuid();
		Parent = parent;
		x = X;
		y = Y;
		loadChunk(x, y);

	}
	void loadChunk(int chunkX, int chunkY)
	{
		for (int x = 0; x < Parent.chunkSize; x++)
		{
			for (int y = 0; y < Parent.chunkSize; y++)
			{
				int c = Parent.standardizedStartingY + (int)(Mathf.PerlinNoise((Parent.seed + (chunkX * Parent.chunkSize) + x) / Parent.scale, (Parent.seed + (chunkY * Parent.chunkSize) + y) / Parent.scale) * Parent.generationHeight);
				Vector3 blockPos = new Vector3((chunkX * Parent.chunkSize) + x, c, (chunkY * Parent.chunkSize) + y);
				blocks.Add(RequestBlock(Parent.grass_block, blockPos));
			}
		}
	}

	public bool isBounds(int playerx, int playery)
	{
		return (int)(playerx / Parent.chunkSize) == x && (int)(playery / Parent.chunkSize) == y;
    }

	public void Clear()
	{
		foreach (GameObject obj in blocks)
		{
			obj.SetActive(false);
			//if (availableBlocks.ContainsKey(obj.tag))
			//{
			//	availableBlocks[obj.tag].Add(obj);
			//}
			//else
			//{
			//	availableBlocks.Add(obj.tag, new List<GameObject>() { obj });
			//}
		}
		blocks.Clear();
	}

	public GameObject RequestBlock(GameObject preFab, Vector3 position)
	{
		if (availableBlocks.ContainsKey(preFab.tag))
		{
			var matches = availableBlocks[preFab.tag];
			if (matches.Count() > 0)
			{
				var perfectMatch = matches.FirstOrDefault(x => x.transform.position == position);
				if (perfectMatch != null)
				{
					perfectMatch.SetActive(true);
					matches.Remove(perfectMatch);
					return perfectMatch;
				}
				var returnData = matches.First();
				returnData.SetActive(true);
				returnData.transform.position = position;
				matches.Remove(returnData);
				return returnData;
			}
		}
		if (availableBlocks.Any() && availableBlocks.First().Value.Any())
		{
			var anyBlock = availableBlocks.First().Value.First();
			anyBlock.SetActive(true);
			anyBlock.transform.position = position;
			availableBlocks.First().Value.Remove(anyBlock);
			return anyBlock;
		}
		return (GameObject)Instantiate(preFab, position, Quaternion.identity);
	}
}
