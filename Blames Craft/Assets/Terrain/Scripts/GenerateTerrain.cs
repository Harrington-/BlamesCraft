using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class GenerateTerrain : MonoBehaviour
{


	[Tooltip("Larger This Height the More Profoud The Hills Will Be")]
	public int generationHeight = 32;

	[Tooltip("Total Blocks Rendered in Each Chunk")]
	public int chunkSize = 16;

	[Tooltip("Total Chunks Rendered in Each Direction")]
	public int renderDistance = 3;

	[Tooltip("General Starting Y for the Map")]
	public int standardizedStartingY = 60;

	[Tooltip("Lower The Value the More Crazy it Gets")]
	public float scale = 1;

	[Tooltip("Lower The Value the More Crazy it Gets")]
	public float seed = 1;

	List<Chunk> chunks;
	private bool chunkChanged;
	public GameObject dirt_block;
	public GameObject sand_block;
	public GameObject grass_block;
	public GameObject stone_block;
	private GameObject player;
	private System.Guid CurrentChunk;

	public int playerChunkX;
	public int playerChunkY;

	// Use this for initialization
	void Start()
	{
		seed = (int)Random.Range(0, 99999999);
		
		player = GameObject.FindGameObjectWithTag("Player");
		chunks = new List<Chunk>();
		for (int i = -renderDistance; i < renderDistance; i++)
		{
			for (int u = -renderDistance; u < renderDistance; u++)
			{
				chunks.Add(new Chunk(this, i, u));
			}
		}
	}

	// Update is called once per frame
	void Update()
	{
		checkPlayerChunkPosition();
		if (chunkChanged)
		{
			reloadChunk();
		}
	}

	void checkPlayerChunkPosition()
	{
		var x = (int)player.transform.position.x;
		var y = (int)player.transform.position.z;
		var chunk = chunks.FirstOrDefault(c => c.isBounds(x, y));
		if (chunk.ChunkId != CurrentChunk)
		{
			CurrentChunk = chunk.ChunkId;
			chunkChanged = true;
		}
	}

	void reloadChunk()
	{
		var chunk = chunks.Single(X => X.ChunkId == CurrentChunk);
		var tempList = new List<Vector2>();
		for (int i = -renderDistance; i < renderDistance; i++)
		{
			for (int u = -renderDistance; u < renderDistance; u++)
			{
				tempList.Add(new Vector2(chunk.x + u, chunk.y + i));
			}
		}
		clearChunks(tempList);
		chunkChanged = false;
	}

	void clearChunks(List<Vector2> neededChunks)
	{
		var newChunks = new List<Chunk>();
		foreach (Chunk chunkie in chunks)
		{
			if(!neededChunks.Any(x=>x.x == chunkie.x && x.y == chunkie.y))
			{
				chunkie.Clear();
			}
			else
			{
				newChunks.Add(chunkie);
				neededChunks.Remove(neededChunks.First(x => x.x == chunkie.x && x.y == chunkie.y));
			}
		}
		foreach(var item in neededChunks)
		{
			newChunks.Add(new Chunk(this, (int)item.x, (int)item.y));
		}
		chunks = newChunks;
	}

	
}
