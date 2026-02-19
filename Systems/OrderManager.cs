#nullable enable
using Microsoft.Xna.Framework;
using Snails.Core;
using Snails.Entities.Items;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Snails.Systems;

public class OrderManager
{
    public List<Order> ActiveOrders { get; } = new();

    private float _spawnTimer;
    private float _spawnInterval;
    private int _totalSpawned;
    private readonly Random _random = new();

    private ItemType[] _allowedDishes;
    private float _spawnIntervalMin;
    private float _spawnIntervalMax;

    public OrderManager(List<string>? allowedDishes = null, float? spawnMin = null, float? spawnMax = null)
    {
        // Convert dish names to ItemType enum
        if (allowedDishes != null && allowedDishes.Count > 0)
        {
            _allowedDishes = allowedDishes
                .Select(name => Enum.Parse<ItemType>(name))
                .ToArray();
        }
        else
        {
            _allowedDishes = new[] { ItemType.Nigiri, ItemType.MakiRoll, ItemType.MisoSoup };
        }

        _spawnIntervalMin = spawnMin ?? GameConstants.OrderSpawnIntervalMin;
        _spawnIntervalMax = spawnMax ?? GameConstants.OrderSpawnIntervalMax;
        _spawnInterval = _spawnIntervalMax;
        _spawnTimer = 5f; // first order comes after 5s
    }

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        _spawnTimer -= dt;
        if (_spawnTimer <= 0 && ActiveOrders.Count < GameConstants.MaxActiveOrders)
        {
            var dish = _allowedDishes[_random.Next(_allowedDishes.Length)];
            ActiveOrders.Add(new Order(dish, GameConstants.OrderTimeLimit));
            _totalSpawned++;

            // Gradually decrease spawn interval
            _spawnInterval = MathHelper.Max(
                _spawnIntervalMin,
                _spawnIntervalMax - _totalSpawned * 0.5f);
            _spawnTimer = _spawnInterval;
        }

        // Update timers and remove expired
        for (int i = ActiveOrders.Count - 1; i >= 0; i--)
        {
            ActiveOrders[i].TimeRemaining -= dt;
            if (ActiveOrders[i].TimeRemaining <= 0)
                ActiveOrders.RemoveAt(i);
        }
    }

    public bool TryFulfillOrder(ItemType itemType)
    {
        var order = ActiveOrders.FirstOrDefault(o => o.RequestedItem == itemType);
        if (order != null)
        {
            ActiveOrders.Remove(order);
            return true;
        }
        return false;
    }
}
