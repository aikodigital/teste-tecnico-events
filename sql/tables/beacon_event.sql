CREATE TABLE IF NOT EXISTS aiko_mdt.beacon_event
(
    date DateTime64(3, 'UTC') CODEC(Delta, ZSTD(1)),
    site_id LowCardinality(String),
    equipment_id Int64 CODEC(Delta, ZSTD(1)),
    source_equipment_id Nullable(Int64) CODEC(Delta, ZSTD(1)),
    source_position_lat Nullable(Float64) CODEC(ZSTD(1)),
    source_position_lon Nullable(Float64) CODEC(ZSTD(1)),
    seen_at Nullable(DateTime64(3, 'UTC')) CODEC(Delta, ZSTD(1)),
    target_beacon Nullable(String) CODEC(ZSTD(3)),
    target_beacon_name Nullable(String) CODEC(ZSTD(3)),
    target_beacon_voltage Nullable(Int32) CODEC(Delta, ZSTD(1)),
    target_beacon_firmware LowCardinality(Nullable(String)),
    target_beacon_model LowCardinality(Nullable(String)),
    target_beacon_hardware LowCardinality(Nullable(String)),
    target_beacon_temperature Nullable(Float32) CODEC(ZSTD(1)),
    target_equipment_id Nullable(Int64) CODEC(Delta, ZSTD(1)),
    target_equipment_state_id Nullable(Int64) CODEC(Delta, ZSTD(1)),
    target_equipment_bearing Nullable(Float32) CODEC(ZSTD(1)),
    target_equipment_speed Nullable(Float32) CODEC(ZSTD(1)),
    target_position_lat Nullable(Float64) CODEC(ZSTD(1)),
    target_position_lon Nullable(Float64) CODEC(ZSTD(1)),
    target_attachment_id Nullable(Int64) CODEC(Delta, ZSTD(1)),
    target_attachment_fraction_id Nullable(Int64) CODEC(Delta, ZSTD(1)),
    signal_strength Nullable(Int32) CODEC(Delta, ZSTD(1)),
    view_count Nullable(Int32) CODEC(Delta, ZSTD(1)),
    repeater_mode Nullable(Bool) CODEC(ZSTD(1)),
    accelerometer_mode Nullable(Bool) CODEC(ZSTD(1)),
    indirect_event Nullable(Bool) CODEC(ZSTD(1)),
    indirect_source_address Nullable(String) CODEC(ZSTD(3)),
    machine_state Nullable(Int32) CODEC(Delta, ZSTD(1)),
    battery Nullable(Int32) CODEC(Delta, ZSTD(1)),
    accelerometer_axis_x Nullable(Int32) CODEC(Delta, ZSTD(1)),
    accelerometer_axis_y Nullable(Int32) CODEC(Delta, ZSTD(1)),
    accelerometer_axis_z Nullable(Int32) CODEC(Delta, ZSTD(1)),
    hash UInt64 CODEC(Delta, ZSTD(1))
)
ENGINE = ReplacingMergeTree()
PARTITION BY toYYYYMM(date)
ORDER BY (site_id, equipment_id, date, hash)
SETTINGS index_granularity = 8192;
