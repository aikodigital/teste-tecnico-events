CREATE TABLE IF NOT EXISTS aiko_mdt.machine_state_event
(
    date DateTime64(3, 'UTC') CODEC(Delta, ZSTD(1)),
    site_id LowCardinality(String),
    equipment_id Int64 CODEC(Delta, ZSTD(1)),
    machine_state Int32 CODEC(Delta, ZSTD(1)),
    hash UInt64 CODEC(Delta, ZSTD(1))
)
ENGINE = ReplacingMergeTree()
PARTITION BY toYYYYMM(date)
ORDER BY (site_id, equipment_id, date, hash)
SETTINGS index_granularity = 8192;
