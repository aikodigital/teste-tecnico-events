CREATE TABLE IF NOT EXISTS aiko_mdt.status_event
(
    date DateTime64(3, 'UTC') CODEC(Delta, ZSTD(1)),
    site_id LowCardinality(String),
    equipment_id Int64 CODEC(Delta, ZSTD(1)),
    location_id Nullable(Int64) CODEC(Delta, ZSTD(1)),
    operator_id Nullable(Int64) CODEC(Delta, ZSTD(1)),
    equipment_state_id Nullable(Int64) CODEC(Delta, ZSTD(1)),
    message_queue_size Int32 CODEC(Delta, ZSTD(1)),
    location JSON,
    longitude Nullable(Float64) CODEC(ZSTD(1)),
    latitude Nullable(Float64) CODEC(ZSTD(1)),
    accuracy Nullable(Float64) CODEC(ZSTD(1)),
    is_valid Bool CODEC(ZSTD(1)),
    signal_strength Nullable(Float64) CODEC(ZSTD(1)),
    battery_level Int32 CODEC(Delta, ZSTD(1)),
    powered Bool CODEC(ZSTD(1)),
    is_working Bool CODEC(ZSTD(1)),
    is_idle Bool CODEC(ZSTD(1)),
    motion_average Float64 CODEC(ZSTD(1)),
    virtual_hourmeter Float64 CODEC(ZSTD(1)),
    hash UInt64 CODEC(Delta, ZSTD(1))
)
ENGINE = ReplacingMergeTree()
PARTITION BY toYYYYMM(date)
ORDER BY (site_id, equipment_id, date, hash)
SETTINGS index_granularity = 8192;
